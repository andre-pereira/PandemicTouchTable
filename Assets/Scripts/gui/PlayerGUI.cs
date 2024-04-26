using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using static ENUMS;
using static Game;

public class PlayerGUI : MonoBehaviour
{

    #region Properties and Fields
    internal bool Waiting = false;
    private const string WaitForYourTurnText = "Wait for your turn.";
    GameGUI gameGui;
    Game game;

    bool _isAnimating = false;
    List<int> _drawnCards = new List<int>();
    internal GameObject movingPawn = null;
    public CardGUIStates cardsState { get; private set; }
    public List<int> selectedCards;
    private GameObject flyLine;
    private GameObject flyLine2;
    private List<PlayerGUI> playersToShareGUI;

    public int Position;

    public ActionTypes ActionSelected;

    Player _player = null;
    public TMPro.TextMeshProUGUI CurrentInstructionText;
    public TMPro.TextMeshProUGUI playerNameText;
    
    public RoleCardDisplay roleCard;
    public Image roleCardBackground { get; private set; }

    public GameObject PlayerCards;
    private List<GameObject> cardsInHand;

    public GameObject ActionsContainer;
    public GameObject MoveAction;
    private Image MoveActionBackground;
    public GameObject FlyAction;
    private Image FlyActionBackground;
    public GameObject CharterAction;
    private Image CharterActionBackground;
    public GameObject TreatAction;
    private Image TreatActionBackground;
    public GameObject ShareAction;
    private Image ShareActionBackground;
    public GameObject FindCureAction;
    private Image FindCureActionBackground;

    public GameObject[] ContextButtons;
    private ContextButtonStates contextButtonState;

    public GameObject [] ForeCastEventCards;
    internal List<int> ForeCastEventCardsIDs = new List<int>();
    internal int ForeCastEventCardSelected = -1;

    public GameObject[] ResourcePlanningEventCardsCities;
    public GameObject[] ResourcePlanningEventCardsEvents;
    public GameObject[] ResourcePlanningEventCardsEpidemic;
    
    internal List<int> ResourcePlanningEventCardsIDs = new List<int>();
    internal int ResourcePlanningEventCardSelected = -1;

    private EventState pInEvent = EventState.NOTINEVENT;
    
    private const int MAX_CARDS = 5;
    private const int MAX_SAME_COLOR_CARDS = 4;

    public bool callToMobilizePending = false;
    public EventState PInEventCard
    {
        get { return pInEvent; }
        set 
        {
            pInEvent = value;
            eventExecuted = false;
        }
    }
    internal bool eventExecuted = false;

    public Player PlayerModel
    {
        get { if (_player == null) _player = PlayerList.playerAtPosition(Position); return _player; }
        set { _player = value; }
    }

    public GameObject getCardInHand(int cardID)
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            if (cardID < 24)
            {
                CityCardDisplay cityCard = cardsInHand[i].GetComponent<CityCardDisplay>();
                if (cityCard != null)
                {
                    if (cityCard.CityCardData.cityID == cardID)
                    {
                        return cardsInHand[i];
                    }
                }
            }
            else
            {
                EventCardDisplay eventCard = cardsInHand[i].GetComponent<EventCardDisplay>();
                if (eventCard != null)
                {
                    if (eventCard.EventCardData.eventID == cardID)
                    {
                        return cardsInHand[i];
                    }
                }
            }
        }
        return null;
    }

    public GameObject getFirstCardInHand()
    {
        if (cardsInHand.Count > 0)
        {
            return cardsInHand[0];
        }
        else return null;
    }

    private int pilotCitySelected = -1;

    public GameObject[] pilotPawnsTagAlong;
    private Pawn pawnPilotSelected = null;
    private HorizontalLayoutGroup layout;

    #endregion

    public void init()
    {
        gameGui = GameGUI.gui;
        game = Game.theGame;
        
        ActionSelected = ActionTypes.None;
        MoveActionBackground = MoveAction.transform.Find("highlight").GetComponent<Image>();
        FlyActionBackground = FlyAction.transform.Find("highlight").GetComponent<Image>();
        CharterActionBackground = CharterAction.transform.Find("highlight").GetComponent<Image>();
        TreatActionBackground = TreatAction.transform.Find("highlight").GetComponent<Image>();
        ShareActionBackground = ShareAction.transform.Find("highlight").GetComponent<Image>();
        FindCureActionBackground = FindCureAction.transform.Find("highlight").GetComponent<Image>();
        roleCardBackground = roleCard.transform.Find("background").GetComponent<Image>();

        _player = null;
        cardsInHand = new List<GameObject>();
        roleCard.RoleCardData = GameGUI.gui.roleCards[(int)PlayerModel.Role];
        roleCard.gameObject.SetActive(true);
        Color targeColor = roleCard.RoleCardData.roleColor;
        targeColor.a = GameGUI.gui.playerUIOpacity;
        GetComponent<Image>().color = targeColor;

        changeContextText();

        playersToShareGUI = new List<PlayerGUI>();

        playerNameText.text = PlayerModel.Name;
        selectedCards = new List<int>();

        layout = PlayerCards.GetComponent<HorizontalLayoutGroup>();
    }

    public void draw()
    {
        if (_isAnimating || PlayerModel == null) return;
        
        createCardsInHand();
        if (_player.PlayerCardsInHand.Count > 6)
        {
            drawHandleDiscard();
        }
        else if (cardsState != CardGUIStates.None || PlayerModel != game.CurrentPlayer)
        {
            if (cardsState == CardGUIStates.CardsDiscarding)
            {
                cardsState = CardGUIStates.None;
            }
            changeHorizontalLayout(option: 2);
        }
        else
        {
            changeHorizontalLayout(option: 3);
        }
        
        if (pInEvent != EventState.NOTINEVENT && _player.PlayerCardsInHand.Count <= 6)
        {
            drawEventHandling();
        }
            else if (PlayerModel == game.CurrentPlayer) ownTurnActionHandling();
            else notMyTurnHandling();

        changeContextText();
    }

    private void notMyTurnHandling()
    {
        enableOwnTurnActions(false);
        if (cardsState == CardGUIStates.CardsExpandedShareAction)
        {
            if (PlayerModel.PlayerCardsInHand.Contains(_player.GetCurrentCity()))
                getCardInHand(PlayerModel.GetCurrentCity()).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
        }
    }

    private void createCardsInHand()
    {
        cardsInHand.Clear();
        PlayerCards.DestroyChildrenImmediate();

        foreach (int cardToAdd in _player.PlayerCardsInHand)
        {
            cardsInHand.Add(game.AddPlayerCardToTransform(cardToAdd, PlayerCards.transform, true, this));
            
        }
    }

    private void drawHandleDiscard()
    {
        changeHorizontalLayout(option: 1);
        cardsState = CardGUIStates.CardsDiscarding;
        if (selectedCards.Count > 0)
        {
            ContextButtons[1].SetActive(false);
            if (selectedCards[0] < 24)
            {
                getCardInHand(selectedCards[0]).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
            }
            else
            {
                getCardInHand(selectedCards[0]).GetComponent<EventCardDisplay>().border.gameObject.SetActive(true);
                ContextButtons[1].SetActive(true);
            }
            ContextButtons[2].SetActive(true);
        }
    }

    private void drawEventHandling()
    {
        if(pInEvent == EventState.CONFIRMINGCALLTOMOBILIZE || pInEvent == EventState.CONFIRMINGRESOURCEPLANNING ||
            pInEvent == EventState.CONFIRMINGMOBILEHOSPITAL|| pInEvent == EventState.CONFIRMINGFORECAST)
        {
            EnableContextButtons(true, true, false, false, false, false);
        }
        else if(pInEvent == EventState.FORECAST)
        {
            EnableContextButtons(false, true, false, true, true, false);

            PlayerCards.SetActive(false);
            roleCard.gameObject.SetActive(false);

            ForeCastEventCards[0].transform.parent.gameObject.SetActive(true);

            for (int i = 0; i < ForeCastEventCards.Length; i++)
            {
                if (i <= ForeCastEventCardsIDs.Count - 1)
                {
                    CityCard infoCard = theGame.Cities[ForeCastEventCardsIDs[i]].city;
                    InfectionCardDisplay cardDisplay = ForeCastEventCards[i].GetComponentInChildren<InfectionCardDisplay>();
                    cardDisplay.CityCardData = infoCard;
                    ForeCastEventCards[i].SetActive(true);

                    if (infoCard.cityID == ForeCastEventCardSelected)
                    {
                        cardDisplay.border.gameObject.SetActive(true);
                    }
                    else
                    {
                        cardDisplay.border.gameObject.SetActive(false);
                    }
                }
                else ForeCastEventCards[i].SetActive(false);
            }

        }
        else if (pInEvent == EventState.CALLTOMOBILIZE)
        {
            EnableContextButtons(false, !eventExecuted, false, false, false, false);
        }
        else if (pInEvent == EventState.EXECUTINGMOBILEHOSPITAL)
        {
            EnableContextButtons(false, false, false, false, false, false);
        }
        else if (pInEvent == EventState.RESOURCEPLANNING)
        {
            EnableContextButtons(false, true, false, true, true, false);

            PlayerCards.SetActive(false);
            roleCard.gameObject.SetActive(false);

            ResourcePlanningEventCardsCities[0].transform.parent.parent.gameObject.SetActive(true);
            
            for (int i = 0; i < ResourcePlanningEventCardsCities.Length; i++)
            {
                if (i <= ResourcePlanningEventCardsIDs.Count - 1)
                {
                    if (ResourcePlanningEventCardsIDs[i] < 24)
                    {
                        ResourcePlanningEventCardsEpidemic[i].SetActive(false);
                        ResourcePlanningEventCardsEvents[i].SetActive(false);
                        
                        CityCard infoCard = theGame.Cities[ResourcePlanningEventCardsIDs[i]].city;
                        CityCardDisplay cardDisplay = ResourcePlanningEventCardsCities[i].GetComponentInChildren<CityCardDisplay>();
                        cardDisplay.CityCardData = infoCard;
                        
                        ResourcePlanningEventCardsCities[i].SetActive(true);

                        if (infoCard.cityID == ResourcePlanningEventCardSelected)
                        {
                            cardDisplay.border.gameObject.SetActive(true);
                        }
                        else
                        {
                            cardDisplay.border.gameObject.SetActive(false);
                        }
                    }
                    else if (ResourcePlanningEventCardsIDs[i] < 28)
                    {
                        ResourcePlanningEventCardsEpidemic[i].SetActive(false);
                        ResourcePlanningEventCardsCities[i].SetActive(false);
                        
                        EventCard infoCard = GameGUI.gui.Events[ResourcePlanningEventCardsIDs[i] - 24];
                        EventCardDisplay cardDisplay = ResourcePlanningEventCardsEvents[i].GetComponentInChildren<EventCardDisplay>();
                        cardDisplay.EventCardData = infoCard;
                        
                        ResourcePlanningEventCardsEvents[i].SetActive(true);
                        
                        if (infoCard.eventID == ResourcePlanningEventCardSelected)
                        {
                            cardDisplay.border.gameObject.SetActive(true);
                        }
                        else
                        {
                            cardDisplay.border.gameObject.SetActive(false);
                        }

                    }
                    else if (ResourcePlanningEventCardsIDs[i] == 28)
                    {
                        ResourcePlanningEventCardsCities[i].SetActive(false);
                        ResourcePlanningEventCardsEvents[i].SetActive(false);

                        EpidemicCardDisplay cardDisplay = ResourcePlanningEventCardsEpidemic[i]
                            .GetComponentInChildren<EpidemicCardDisplay>();
                        
                        ResourcePlanningEventCardsEpidemic[i].SetActive(true);
                        
                        if (ResourcePlanningEventCardSelected == 28)
                        {
                            cardDisplay.border.gameObject.SetActive(true);
                        }
                        else
                        {
                            cardDisplay.border.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void EnableContextButtons(bool first, bool second, bool third, bool fourth, bool fifth, bool sixth)
    {
        ContextButtons[0].SetActive(first);
        ContextButtons[1].SetActive(second);
        ContextButtons[2].SetActive(third);
        ContextButtons[3].SetActive(fourth);
        ContextButtons[4].SetActive(fifth);
        ContextButtons[5].SetActive(sixth);
    }

    private void ownTurnActionHandling()
    {
        bool moveAction = false;
        bool flyAction = false;
        bool charterAction = false;
        bool findCureAction = false;
        bool treatAction = false;
        bool shareAction = false;

        //changeContextText(true);
        if (PlayerModel.ActionsRemaining > 0 && !Waiting)
        {
            if (cardsState == CardGUIStates.None)
            {
                moveAction = true;
                List<int>[] cardsOfEachColor = new List<int>[3];
                foreach (int card in PlayerModel.CityCardsInHand)
                {
                    if (card != PlayerModel.GetCurrentCity())
                        flyAction = true;
                    else
                        charterAction = true;
                }

                if (PlayerModel.GetCurrentCityScript().cubesInCity() || (_player.Role == Player.Roles.Virologist 
                                                                         && !_player.secondRoleActionUsed)) 
                {
                    treatAction = true;
                }

                if (ableToFindCure())
                    if (PlayerModel.GetCurrentCity() == game.InitialCityID)
                        findCureAction = true;

                playersToShareGUI.Clear();

                int countOtherPlayerInCity = 0;
                foreach (Player player in PlayerModel.GetCurrentCityScript().PlayersInCity)
                {
                    if (player != _player)
                    {
                        countOtherPlayerInCity++;
                        if (player.PlayerCardsInHand.Contains(_player.GetCurrentCity()) || _player.PlayerCardsInHand.Contains(_player.GetCurrentCity()))
                        {
                            shareAction = true;
                            playersToShareGUI.Add(GameGUI.playerPadForPosition(player.Position));
                        }
                    }
                }
            }
            else
            {
                if (cardsState == CardGUIStates.CardsExpandedFlyActionToSelect || cardsState == CardGUIStates.CardsExpandedCharterActionToSelect
                    || cardsState == CardGUIStates.CardsExpandedCureActionToSelect || cardsState == CardGUIStates.CardsExpandedShareAction
                    || cardsState == CardGUIStates.CardsExpandedVirologistAction)
                {
                    ContextButtons[0].SetActive(true);
                    if (cardsState == CardGUIStates.CardsExpandedCharterActionToSelect || cardsState == CardGUIStates.CardsExpandedShareAction)
                    {
                        getCardInHand(PlayerModel.GetCurrentCity()).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
                    }
                }
            }
        }

        MoveAction.SetActive(moveAction);
        FlyAction.SetActive(flyAction);
        TreatAction.SetActive(treatAction);
        CharterAction.SetActive(charterAction);
        FindCureAction.SetActive(findCureAction);
        ShareAction.SetActive(shareAction);
    }

    private void changeHorizontalLayout(int option)
    {
        if (option == 1)
        {
            layout.gameObject.transform.localScale = new Vector3(0.86f, 0.86f, 1f);
            layout.padding.left = -695;
            layout.spacing = 31f;
            layout.childForceExpandWidth = false;
            layout.childControlWidth = false;
        }
        else if (option == 2)
        {
            layout.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            layout.spacing = 30;
            layout.padding.left = -585;
            layout.childForceExpandWidth = false;
            layout.childControlWidth = false;
        }
        else if (option == 3)
        {
            layout.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            layout.spacing = 22.5f;
            layout.padding.left = -15;
            layout.childForceExpandWidth = true;
            layout.childControlWidth = true;
        }
    }

    private bool ableToFindCure()
    {
        return (!game.RedCure && PlayerModel.RedCardsInHand.Count > 3) ||
               (!game.YellowCure && PlayerModel.YellowCardsInHand.Count > 3) ||
               (!game.BlueCure && PlayerModel.BlueCardsInHand.Count > 3) ||
               isVirologistAbleToCure();
    }

    private bool isVirologistAbleToCure()
    {
        return _player.Role == Player.Roles.Virologist && PlayerModel.CityCardsInHand.Count >= 5 && (
            (!game.RedCure && PlayerModel.RedCardsInHand.Count == 3) ||
            (!game.YellowCure && PlayerModel.YellowCardsInHand.Count == 3) ||
            (!game.BlueCure && PlayerModel.BlueCardsInHand.Count == 3)
        );
    }

    #region Buttons

    public void ForecastInfectionCardClicked(int position)
    {
        ForeCastEventCardSelected = ForeCastEventCardsIDs[position];
        draw();
    }

    public void ResourcePlanningInfectionCardClicked(int position)
    {
        ResourcePlanningEventCardSelected = ResourcePlanningEventCardsIDs[position];
        draw();
    }

    public void ContextButtonClicked(int buttonType)
    {
        if(buttonType == 2)
        {
            DiscardButtonClicked();
        }
        if (buttonType == 1)
        {
            AcceptButtonClicked();
        }

        if (buttonType == 0)
        {
            CloseButtonClicked();
        }

        if (this != GameGUI.currentPlayerPad() && pInEvent == EventState.NOTINEVENT)
        {
            GameGUI.currentPlayerPad().ClearSelectedAction();
            GameGUI.currentPlayerPad().draw();
        }

        if (buttonType == 3)
        {
            LeftArrowButtonClicked();
        }

        if (buttonType == 4)
        {
            RightArrowButtonClicked();
        }

        cardsState = CardGUIStates.None;
        ClearSelectedAction();
        draw();
    }

    private void RightArrowButtonClicked()
    {
        ContextButtons[5].SetActive(false);
        if(pInEvent == EventState.FORECAST)
        {
            int index = ForeCastEventCardsIDs.IndexOf(ForeCastEventCardSelected);

            int temp = ForeCastEventCardsIDs[index];
            if (index == ForeCastEventCardsIDs.Count - 1)
            {
                ForeCastEventCardsIDs.RemoveAt(index);
                ForeCastEventCardsIDs.Insert(0, temp);
            }
            else
            {
                ForeCastEventCardsIDs[index] = ForeCastEventCardsIDs[index + 1];
                ForeCastEventCardsIDs[index + 1] = temp;
            }
        }
        else if (pInEvent == EventState.RESOURCEPLANNING)
        {
            int index = ResourcePlanningEventCardsIDs.IndexOf(ResourcePlanningEventCardSelected);

            int temp = ResourcePlanningEventCardsIDs[index];
            if (index == ResourcePlanningEventCardsIDs.Count - 1)
            {
                ResourcePlanningEventCardsIDs.RemoveAt(index);
                ResourcePlanningEventCardsIDs.Insert(0, temp);
            }
            else
            {
                ResourcePlanningEventCardsIDs[index] = ResourcePlanningEventCardsIDs[index + 1];
                ResourcePlanningEventCardsIDs[index + 1] = temp;
            }
        }
        draw();
    }

    private void LeftArrowButtonClicked()
    {
        ContextButtons[5].SetActive(false);
        if (pInEvent == EventState.FORECAST)
        {
            int index = ForeCastEventCardsIDs.IndexOf(ForeCastEventCardSelected);

            int temp = ForeCastEventCardsIDs[index];
            if (index == 0)
            {
                ForeCastEventCardsIDs.RemoveAt(index);
                ForeCastEventCardsIDs.Add(temp);
            }
            else
            {
                ForeCastEventCardsIDs[index] = ForeCastEventCardsIDs[index - 1];
                ForeCastEventCardsIDs[index - 1] = temp;
            }
        }
        else if (pInEvent == EventState.RESOURCEPLANNING)
        {
            int index = ResourcePlanningEventCardsIDs.IndexOf(ResourcePlanningEventCardSelected);

            int temp = ResourcePlanningEventCardsIDs[index];
            if (index == 0)
            {
                ResourcePlanningEventCardsIDs.RemoveAt(index);
                ResourcePlanningEventCardsIDs.Add(temp);
            }
            else
            {
                ResourcePlanningEventCardsIDs[index] = ResourcePlanningEventCardsIDs[index - 1];
                ResourcePlanningEventCardsIDs[index - 1] = temp;
            }
        }
        draw();
    }

    private void CloseButtonClicked()
    {
        if (ActionSelected == ActionTypes.Share)
        {
            foreach (PlayerGUI playerGUI in playersToShareGUI)
            {
                playerGUI.ActionSelected = ActionTypes.None;
                playerGUI.ContextButtonClicked(0);

            }
        }
        PInEventCard = EventState.NOTINEVENT;
    }

    private void AcceptButtonClicked()
    {
        if (pInEvent == EventState.CONFIRMINGCALLTOMOBILIZE) Timeline.theTimeline.addEvent(new PCallToMobilizeCardPlayed(PlayerModel));
        else if (pInEvent == EventState.CONFIRMINGRESOURCEPLANNING) Timeline.theTimeline.addEvent(new PResourcePlanningCardPlayed(PlayerModel));
        else if (pInEvent == EventState.CONFIRMINGMOBILEHOSPITAL) Timeline.theTimeline.addEvent(new PMobileHospitalCardPlayed(PlayerModel));
        else if (pInEvent == EventState.CONFIRMINGFORECAST) Timeline.theTimeline.addEvent(new PForecastCardPlayed(PlayerModel));
        
        else if (pInEvent == EventState.NOTINEVENT && cardsState == CardGUIStates.CardsDiscarding)
        {
            if (selectedCards[0] == 24) Timeline.theTimeline.addEvent(new PCallToMobilizeCardPlayed(PlayerModel));
            if (selectedCards[0] == 25) Timeline.theTimeline.addEvent(new PForecastCardPlayed(PlayerModel));
            if (selectedCards[0] == 26) Timeline.theTimeline.addEvent(new PMobileHospitalCardPlayed(PlayerModel));
            if (selectedCards[0] == 27) Timeline.theTimeline.addEvent(new PResourcePlanningCardPlayed(PlayerModel));
        }
        
        else if (pInEvent == EventState.CALLTOMOBILIZE)
        {
            eventExecuted = true;
            DestroyMovingPawn();
            draw();
        }
        else if (pInEvent == EventState.FORECAST)
        {
            Timeline.theTimeline.addEvent(new PForecast(PlayerModel));
            return;
        }
        else if (pInEvent == EventState.RESOURCEPLANNING)
        {
            Timeline.theTimeline.addEvent(new PResourcePlanning(PlayerModel));
            return;
        }

        if (cardsState == CardGUIStates.CardsExpandedFlyActionSelected)
        {
            Timeline.theTimeline.addEvent(new PFlyToCity(selectedCards[0]));
        }
        else if (cardsState == CardGUIStates.CardsExpandedCureActionSelected)
        {
            Timeline.theTimeline.addEvent(new PCureDisease(selectedCards));
        }
        else if (cardsState == CardGUIStates.CardsExpandedShareAction)
        {
            if (this != GameGUI.currentPlayerPad())
            {
                if (PlayerModel.PlayerCardsInHand.Contains(PlayerModel.GetCurrentCity()))
                    Timeline.theTimeline.addEvent(new PShareKnowledge(this, GameGUI.currentPlayerPad()));
                else Timeline.theTimeline.addEvent(new PShareKnowledge(GameGUI.currentPlayerPad(), this));
            }
        }
        else if (ActionSelected == ActionTypes.CharacterAction && PlayerModel.Role == Player.Roles.Pilot)
        {
            if (pawnPilotSelected != null)
            {
                Timeline.theTimeline.addEvent(new PPilotFlyToCity(pilotCitySelected, pawnPilotSelected.PlayerModel));
            }
            else
            {
                Timeline.theTimeline.addEvent(new PPilotFlyToCity(pilotCitySelected, null));
            }
        }
    }

    private void DiscardButtonClicked()
    {
        Timeline.theTimeline.addEvent(new PDiscardCard(selectedCards[0], this));
    }

    public void ActionButtonClicked(int action)
    {
        Debug.Log("Action button clicked: " + action);
        if (PlayerModel == null) return;
        if (PlayerModel != game.CurrentPlayer) return;
        if (PlayerModel.ActionsRemaining <= 0) return;

        ClearSelectedAction(!(action == 6));

        City currentCity = game.Cities[PlayerModel.GetCurrentCity()];
        switch (action)
        {
            case 0: //move
                ActionSelected = ActionTypes.Move;
                MoveActionBackground.color = new Color(1f, 1f, 1f, .25f);
                CreateMovingPawn();
                break;
            case 1: //fly
                ActionSelected = ActionTypes.Fly;
                FlyActionBackground.color = new Color(1f, 1f, 1f, .25f);
                cardsState = CardGUIStates.CardsExpandedFlyActionToSelect;
                draw();
                break;
            case 2: //charter
                ActionSelected = ActionTypes.Charter;
                CharterActionBackground.color = new Color(1f, 1f, 1f, .25f);
                cardsState = CardGUIStates.CardsExpandedCharterActionToSelect;
                CreateMovingPawn();
                draw();
                break;
            case 3: //treat
                ActionSelected = ActionTypes.Treat;
                TreatActionBackground.color = new Color(1f, 1f, 1f, .25f);
                break;
            case 4: //share
                ActionSelected = ActionTypes.Share;
                ShareActionBackground.color = new Color(1f, 1f, 1f, .25f);
                cardsState = CardGUIStates.CardsExpandedShareAction;

                if (PlayerModel.PlayerCardsInHand.Contains(_player.GetCurrentCity()))
                {
                    cardsState = CardGUIStates.CardsExpandedShareAction;
                    draw();
                    
                }
                foreach (PlayerGUI playerGUI in playersToShareGUI)
                {
                    playerGUI.cardsState = CardGUIStates.CardsExpandedShareAction;
                    playerGUI.EnableContextButtons(true, true, false, false, false, false);
                    if (playerGUI.PlayerModel.PlayerCardsInHand.Contains(_player.GetCurrentCity()))
                        playerGUI.getCardInHand(PlayerModel.GetCurrentCity()).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
                    playerGUI.changeContextText();
                    playerGUI.draw();
                }
                break;
            case 5: //find cure
                ActionSelected = ActionTypes.FindCure;
                FindCureActionBackground.color = new Color(1f, 1f, 1f, .25f);
                cardsState = CardGUIStates.CardsExpandedCureActionToSelect;
                draw();
                break;
            case 6: //character actionbv1

                if (_player.Role == Player.Roles.Virologist || _player.Role == Player.Roles.Pilot)
                {
                    Debug.Log("Character action clicked: " + _player.Role);
                    bool enableAction = true;

                    if (_player.Role == Player.Roles.Virologist)
                    {
                        if (PlayerModel.CityCardsInHand.Count == 0 || PlayerModel.roleActionUsed) enableAction = false;
                        else
                        {
                            cardsState = CardGUIStates.CardsExpandedVirologistAction;
                            draw();
                        }
                    }

                    if (enableAction)
                    {
                        ActionSelected = ActionTypes.CharacterAction;
                        roleCardBackground.GetComponent<Outline>().enabled = true;
                    }
                }
                break;
        }
        changeContextText();
    }

    public void CardInHandClicked(int cardClicked)
    {
        if(Waiting) return;

        CityCardDisplay cardClickedScript = null;
        EventCardDisplay eventCardDisplay = null;

        if (cardClicked < 24)
            cardClickedScript = getCardInHand(cardClicked).GetComponent<CityCardDisplay>();
        else 
            eventCardDisplay = getCardInHand(cardClicked).GetComponent<EventCardDisplay>();

        if (cardsState == CardGUIStates.CardsDiscarding)
        {
            selectedCards.Clear();
            selectedCards.Add(cardClicked);
            draw();
        }
        if (_player == game.CurrentPlayer)
        {
            if (cardsState == CardGUIStates.None)
            {
                cardsState = CardGUIStates.CardsExpanded;
                ContextButtons[0].SetActive(true);
                draw();
                return;
            }
        }
        if (cardClicked < 24)
        {
            if (cardsState == CardGUIStates.CardsExpandedCharterActionToSelect)
            {
                if (cardClicked != _player.GetCurrentCity())
                {
                    return;
                }
                else
                {
                    selectedCards.Add(cardClicked);
                    if (flyLine != null)
                    {
                        Destroy(flyLine);
                    }
                    cardClickedScript.border.gameObject.SetActive(true);
                }
            }

            if (cardsState == CardGUIStates.CardsExpandedFlyActionToSelect || cardsState == CardGUIStates.CardsExpandedFlyActionSelected)
            {
                cardsState = CardGUIStates.CardsExpandedFlyActionSelected;
                selectedCards.Clear();
                selectedCards.Add(cardClicked);

                if (cardClickedScript != null)
                {
                    if (flyLine != null)
                    {
                        Destroy(flyLine);
                    }

                    removeBorderFromCardsInHand();
                    cardClickedScript.border.gameObject.SetActive(true);
                    City cityToMoveTo = game.Cities[cardClicked];
                    City cityToMoveFrom = game.Cities[_player.GetCurrentCity()];

                    cityToMoveFrom.PawnsInCity[_player.Position].gameObject.GetComponent<Outline>().enabled = true;
                    CreateLineBetweenCities(cityToMoveTo, cityToMoveFrom);
                    ContextButtons[1].SetActive(true);
                }
            }

            if (cardsState == CardGUIStates.CardsExpandedCureActionToSelect || cardsState == CardGUIStates.CardsExpandedCureActionSelected)
            {
                if (selectedCards.Contains(cardClicked))
                {
                    selectedCards.Remove(cardClicked);
                    getCardInHand(cardClicked).GetComponent<CityCardDisplay>().border.gameObject.SetActive(false);
                    ContextButtons[1].SetActive(false);
                }
                else
                {
                    if (AddCardAndTestForCure(cardClickedScript)) //TODO: check how this works
                    { 
                            Debug.Log("You can now cure the disease!");
                            cardsState = CardGUIStates.CardsExpandedCureActionSelected;
                            ContextButtons[1].SetActive(true);
                    }

                }
            }
        }
        else
        {
            if (pInEvent == EventState.NOTINEVENT && cardsState != CardGUIStates.CardsDiscarding)
            {
                if (cardClicked == 24)      pInEvent = EventState.CONFIRMINGCALLTOMOBILIZE;
                else if (cardClicked == 25) pInEvent = EventState.CONFIRMINGFORECAST;
                else if (cardClicked == 26) pInEvent = EventState.CONFIRMINGMOBILEHOSPITAL;
                else if (cardClicked == 27) pInEvent = EventState.CONFIRMINGRESOURCEPLANNING;
                if(pInEvent != EventState.NOTINEVENT)
                {
                    selectedCards.Add(cardClicked);
                    draw();
                }
            }
        }


    }

    public void CityClicked(City city)
    {
        if (ActionSelected == ActionTypes.Treat && 
            _player.GetCurrentCity() == city.city.cityID &&
            _player.ActionsRemaining > 0 && city.cubesInCity())
        {

            Timeline.theTimeline.addEvent(new PTreatDisease(city));

        } else if (ActionSelected == ActionTypes.Treat && _player.Role == Player.Roles.Virologist 
                   && _player.ActionsRemaining > 0 && city.cubesInCity() && !_player.secondRoleActionUsed) {

            Debug.Log("Treating City :", city);

            VirusName? virusFound = city.firstVirusFoundInCity();
            if (virusFound.HasValue)
            {
                VirusName virusColor = virusFound.Value;
                Debug.Log("Virus color found in the city: " + virusColor);

                if ((virusColor == VirusName.Red && _player.RedCardsInHand.Any()) ||
                    (virusColor == VirusName.Yellow && _player.YellowCardsInHand.Any()) ||
                    (virusColor == VirusName.Blue && _player.BlueCardsInHand.Any()))
                {
                    _player.secondRoleActionUsed = true;
                    Timeline.theTimeline.addEvent(new PTreatDisease(city));
                }
            }

            /*Debug.Log("Red Cards in hand ? " + _player.RedCardsInHand.Any());
            Debug.Log("Yellow Cards in hand ? " + _player.YellowCardsInHand.Any());
            Debug.Log("Blue Cards in hand ? " + _player.BlueCardsInHand.Any());*/
        }

        else if (ActionSelected == ActionTypes.CharacterAction && PlayerModel.Role == Player.Roles.Pilot)
        {
            City cityToMoveTo = game.Cities[city.city.cityID];
            City cityToMoveFrom = game.Cities[_player.GetCurrentCity()];

            int distance = game.DistanceFromCity(cityToMoveFrom.city.cityID, cityToMoveTo.city.cityID);

            if (distance > 0 && distance < 3)
            {
                pilotCitySelected = city.city.cityID;

                if (flyLine != null) Destroy(flyLine);
                if (flyLine2 != null) Destroy(flyLine2);

                cityToMoveFrom.PawnsInCity[_player.Position].gameObject.GetComponent<Outline>().enabled = true;
                CreateLineBetweenCities(cityToMoveTo, cityToMoveFrom);

                int counterPlayers = 0;
                foreach (Player player in cityToMoveFrom.PlayersInCity)
                {
                    if (player != PlayerModel)
                    {
                        GameObject pawn = pilotPawnsTagAlong[counterPlayers];
                        pawn.SetActive(true);
                        Pawn pawnScript = pawn.GetComponent<Pawn>();
                        pawnScript.SetRoleAndPlayer(player);
                        pawnScript.IsInterfaceElement = true;
                        counterPlayers++;
                    }
                }

                if (pawnPilotSelected != null)
                    CreateLineBetweenGameObjects(cityToMoveTo.gameObject, getPawnInCurrentCity(pawnPilotSelected).gameObject, gameGui.roleCards[(int)pawnPilotSelected.PawnRole]);

                EnableContextButtons(true, true, false, false, false, false);
                changeContextText();
            }

        }
    }

    internal void CubeClicked(City city, VirusName virusName)
    {
        if (ActionSelected == ActionTypes.Treat && _player.GetCurrentCity() == city.city.cityID 
            && _player.ActionsRemaining > 0 && city.cubesInCity())
        {
            Timeline.theTimeline.addEvent(new PTreatDisease(city, virusName));
        }
        else if( ActionSelected == ActionTypes.Treat && _player.Role == Player.Roles.Virologist 
                                                     && _player.ActionsRemaining > 0 
                                                     && city.cubesInCity() 
                                                     && (!PlayerModel.secondRoleActionUsed))
        {
            foreach (int card in PlayerModel.CityCardsInHand)
            {
                if (game.Cities[card].city.virusInfo.virusName == virusName)
                {
                    Timeline.theTimeline.addEvent(new PTreatDisease(city, virusName));
                    PlayerModel.secondRoleActionUsed = true;
                    break;
                }
            }
        }
    }

    internal void PawnClicked(Pawn pawn)
    {
        foreach (GameObject pawnObject in pilotPawnsTagAlong)
        {
            pawnObject.GetComponent<Outline>().enabled = false;
        }

        if (flyLine2 != null)
        {
            Destroy(flyLine2);
        }

        if (pawn != pawnPilotSelected)
        {
            pawn.GetComponent<Outline>().enabled = true;
            pawnPilotSelected = pawn;
            CreateLineBetweenGameObjects(game.Cities[pilotCitySelected].gameObject, getPawnInCurrentCity(pawn).gameObject, gameGui.roleCards[(int)pawn.PawnRole]);
        }
        else
        {
            pawnPilotSelected = null;

        }
        changeContextText();
    }

    private Pawn getPawnInCurrentCity(Pawn pawn)
    {
        foreach (Pawn pawnInCity in PlayerModel.GetCurrentCityScript().PawnsInCity)
        {
            if(pawnInCity.PawnRole == pawn.PawnRole)
                return pawnInCity;
        }
        return null;
    }

    #endregion

    private void CountSelectedCardColors(out int numRedCards, out int numYellowCards, out int numBlueCards)
    {
        numRedCards = 0;
        numBlueCards = 0;
        numYellowCards = 0;
        
        foreach (int card in selectedCards)
        {
            switch (game.Cities[card].city.virusInfo.virusName)
            {
                case VirusName.Red:
                    numRedCards++;
                    break;
                case VirusName.Yellow:
                    numYellowCards++;
                    break;
                case VirusName.Blue:
                    numBlueCards++;
                    break;
            }
        }
    }
    /*
     * Check whether the disease can be cured after selecting a card
     * Returns true when curing the disease is possible, false otherwise
     */
    private bool AddCardAndTestForCure(CityCardDisplay cardClickedScript)
    {
        bool isVirologist = PlayerModel.Role == Player.Roles.Virologist;
        VirusName cardClickedVirusName = cardClickedScript.CityCardData.virusInfo.virusName;

        int cardID = cardClickedScript.CityCardData.cityID;
        

        CountSelectedCardColors(out int numRedCards, out int numYellowCards, out int numBlueCards);
        
        if (CanAddCard(cardClickedVirusName, numRedCards, numBlueCards, numYellowCards, isVirologist))
        {
            selectedCards.Add(cardID);
            switch (cardClickedVirusName)
            {
                case VirusName.Red:
                    numRedCards++;
                    break;
                case VirusName.Blue:
                    numBlueCards++;
                    break;
                case VirusName.Yellow:
                    numYellowCards++;
                    break;
            }
            getCardInHand(cardID).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
        }
        
        // Test if curing the disease if possible
        return TestForCure(numRedCards, numBlueCards, numYellowCards, isVirologist);
    }

    private bool CanAddCard(VirusName cardClickedVirusName, int numRedCards, int numBlueCards,
        int numYellowCards, bool isVirologist)
    {
        
        int numCardsOfClickedColor =
            GetNumCardsOfClickedColor(cardClickedVirusName, numRedCards, numBlueCards, numYellowCards);
        int numCardsOfOtherColor =
            GetNumCardsOfOtherColors(cardClickedVirusName, numRedCards, numBlueCards, numYellowCards);

        int numOtherColors = 0;
        switch (cardClickedVirusName)
        {
            case VirusName.Blue:
                if (numYellowCards > 0) numOtherColors++;
                if (numRedCards > 0) numOtherColors++;
                break;
            case VirusName.Red:
                if (numBlueCards > 0) numOtherColors++;
                if (numYellowCards > 0) numOtherColors++;
                break;
            case VirusName.Yellow:
                if (numRedCards > 0) numOtherColors++;
                if (numBlueCards > 0) numOtherColors++;
                break;
        }

        if (isVirologist)
        {
            if (numCardsOfClickedColor == 2 && IsCureAlreadyFound(cardClickedVirusName))
            {
                Debug.Log($"The {cardClickedVirusName} disease has already been cured." +
                          $" As a virologist, you can't pick more than 2 of its cards");
                return false;
            }
        }
        else
        {
            if (IsCureAlreadyFound(cardClickedVirusName))
            {
                Debug.Log($"Cure has already been found for {cardClickedVirusName}");
                return false;
            }
        }

        bool canAdd;
        if (numCardsOfClickedColor == MAX_SAME_COLOR_CARDS && (numCardsOfOtherColor == 0)) canAdd = false;
        else
        {
            if (isVirologist)
            {
               /* canAdd = ((numCardsOfClickedColor + numCardsOfOtherColor <= MAX_CARDS) 
                          && (numCardsOfClickedColor != MAX_SAME_COLOR_CARDS - 1 || numCardsOfOtherColor != 2)) 
                         || ((numCardsOfClickedColor < MAX_SAME_COLOR_CARDS) && (numCardsOfOtherColor == 0));*/
               canAdd = ((numCardsOfClickedColor + numCardsOfOtherColor < MAX_CARDS) 
                         && !((numCardsOfClickedColor == 3 && numCardsOfOtherColor == 2)
                              || (numCardsOfClickedColor == 0 && numCardsOfOtherColor == 4 && numOtherColors == 1) 
                              //numCardsOfClickedColor is updated when the card is added to selectedCards
                              //numOtherColors is the number of colors of numCardsOfOtherColor
                              || (numCardsOfClickedColor == 2 && numCardsOfOtherColor == 3)
                              || (numCardsOfClickedColor == 4 && numCardsOfOtherColor == 0)))
                         || ((numCardsOfClickedColor < MAX_SAME_COLOR_CARDS) && (numCardsOfOtherColor == 0));
                Debug.Log($"Virologist {cardClickedVirusName}, card addition branch, canAdd: {canAdd}, " +
                          $"numColor : {numOtherColors}");
            }
            else
            {
                canAdd = (numCardsOfClickedColor < MAX_SAME_COLOR_CARDS) && (numCardsOfOtherColor == 0);
            }
        }
        
        return canAdd;
    }

    private bool TestForCure(int numRedCards, int numBlueCards, int numYellowCards, bool isVirologist)
    {
        bool isCurePossible = false;
        if (!isVirologist)
        {
            if (selectedCards.Count == MAX_SAME_COLOR_CARDS) isCurePossible = true;
        }
        else
        {
            if ((selectedCards.Count == MAX_CARDS && (numRedCards == 3 && numBlueCards + numYellowCards == 2)
                    || (numBlueCards == 3 && numRedCards + numYellowCards == 2)
                    || (numYellowCards == 3 && numRedCards + numBlueCards == 2)) 
                || (selectedCards.Count == MAX_SAME_COLOR_CARDS &&
                    (numRedCards == MAX_SAME_COLOR_CARDS 
                     || numYellowCards == MAX_SAME_COLOR_CARDS 
                     || numBlueCards == MAX_SAME_COLOR_CARDS)))
            {
                isCurePossible = true; 
            }
        }
        return isCurePossible;
    }
    
    private bool IsCureAlreadyFound(VirusName virusName)
    {
        bool isCureFound = false;
        switch (virusName)
        {
            case VirusName.Red:
                isCureFound = game.RedCure;
                break;
            case VirusName.Blue:
                isCureFound = game.BlueCure;
                break;
            case VirusName.Yellow:
                isCureFound = game.YellowCure;
                break;
        }

        return isCureFound;
    }

    private int GetNumCardsOfClickedColor(VirusName virusName, int numRedCards, int numBlueCards, int numYellowCards)
    {
        int numCardsOfClickedColor = 0;
        switch (virusName)
        {
            case VirusName.Red:
                numCardsOfClickedColor = numRedCards;
                break;
            case VirusName.Blue:
                numCardsOfClickedColor = numBlueCards;
                break;
            case VirusName.Yellow:
                numCardsOfClickedColor = numYellowCards;
                break;
        }

        return numCardsOfClickedColor;
    }

    private int GetNumCardsOfOtherColors(VirusName virusName, int numRedCards, int numBlueCards, int numYellowCards)
    {
        int numCardsOfOtherColors = 0;
        switch (virusName)
        {
            case VirusName.Red:
                numCardsOfOtherColors = numBlueCards + numYellowCards;
                break;
            case VirusName.Blue:
                numCardsOfOtherColors = numRedCards + numYellowCards;
                break;
            case VirusName.Yellow:
                numCardsOfOtherColors = numBlueCards + numRedCards;
                break;
        }

        return numCardsOfOtherColors;
    }
    
    private void enableOwnTurnActions(bool enabled)
    {
        MoveAction.SetActive(enabled);
        FlyAction.SetActive(enabled);
        CharterAction.SetActive(enabled);
        TreatAction.SetActive(enabled);
        ShareAction.SetActive(enabled);
        FindCureAction.SetActive(enabled);
    }

    public void CreateMovingPawn(Vector3? translation = null)
    {
        City currentCity = PlayerModel.GetCurrentCityScript();
        currentCity.removePawn(PlayerModel);
        currentCity.draw();
        if(translation == null)
            movingPawn = Instantiate(gameGui.PawnPrefab, currentCity.transform.position, currentCity.transform.rotation, gameGui.AnimationCanvas.transform);
        else
            movingPawn = Instantiate(gameGui.PawnPrefab, currentCity.transform.position + (Vector3)translation, currentCity.transform.rotation, gameGui.AnimationCanvas.transform);
        movingPawn.GetComponent<Pawn>().CanMove = true;
        movingPawn.GetComponent<Pawn>().SetRoleAndPlayer(PlayerModel);
        movingPawn.GetComponent<Outline>().enabled = true;
    }

    public void ClearSelectedAction(bool clear = true)
    {
        selectedCards.Clear();

        if (flyLine != null)
        {
            Destroy(flyLine);
        }

        if (movingPawn != null)
        {
            DestroyMovingPawn();
        }
        MoveActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        FlyActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        CharterActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        TreatActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        ShareActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        FindCureActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        roleCardBackground.GetComponent<Outline>().enabled = false;

        ActionSelected = ActionTypes.None;
        cardsState = CardGUIStates.None;

        if (clear) EnableContextButtons(false, false, false, false, false, false);

        pilotCitySelected = -1;
        if (flyLine != null) Destroy(flyLine);
        flyLine = null;

        if (flyLine2 != null) Destroy(flyLine2);
        flyLine2 = null;

        pawnPilotSelected = null;
        foreach (GameObject pawn in pilotPawnsTagAlong)
        {
            pawn.GetComponent<Outline>().enabled = false;
            pawn.SetActive(false);
        }

        roleCard.gameObject.SetActive(true);
        ActionsContainer.SetActive(true);
        PlayerCards.SetActive(true);

        changeContextText();
    }

    public void DestroyMovingPawn()
    {
        Destroy(movingPawn);
        movingPawn = null;
        City currentCity = game.Cities[PlayerModel.GetCurrentCity()];
        currentCity.addPawn(PlayerModel);
        currentCity.draw();
    }

    private void CreateLineBetweenCities(City cityToMoveTo, City cityToMoveFrom)
    {
        flyLine = new GameObject("Line - FlyAction");
        flyLine.transform.SetParent(gameGui.AnimationCanvas.transform, false);
        flyLine.transform.position = cityToMoveFrom.PawnsInCity[_player.Position].transform.position;
        flyLine.AddComponent<LineRenderer>();
        LineRenderer lr = flyLine.GetComponent<LineRenderer>();
        lr.sortingLayerName = "Animation";
        lr.material = gameGui.lineMaterial;
        lr.startColor = roleCard.RoleCardData.roleColor;
        lr.endColor = roleCard.RoleCardData.roleColor;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, cityToMoveFrom.PawnsInCity[_player.Position].transform.position);
        lr.SetPosition(1, cityToMoveTo.transform.position);
    }

    private void CreateLineBetweenGameObjects(GameObject one, GameObject two, RoleCard roleData)
    {
        flyLine2 = new GameObject("Line - FlyAction");
        flyLine2.transform.SetParent(gameGui.AnimationCanvas.transform, false);
        flyLine2.transform.position = one.transform.position;
        flyLine2.AddComponent<LineRenderer>();
        LineRenderer lr = flyLine2.GetComponent<LineRenderer>();
        lr.sortingLayerName = "Animation";
        lr.material = gameGui.lineMaterial;
        lr.startColor = roleData.roleColor;
        lr.endColor = roleData.roleColor;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, one.transform.position);
        lr.SetPosition(1, two.transform.position);
    }

    private void removeBorderFromCardsInHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CityCardDisplay cityCard = cardsInHand[i].GetComponent<CityCardDisplay>();
            if (cityCard != null)
            {
                cityCard.border.gameObject.SetActive(false);
            }
            else
            {

                EventCardDisplay eventCard = cardsInHand[i].GetComponent<EventCardDisplay>();
                if (eventCard != null)
                {
                    eventCard.border.gameObject.SetActive(false);
                }
            }
        }
    }

    private void changeContextText()
    {
        if (Waiting && pInEvent != EventState.EXECUTINGMOBILEHOSPITAL)
        {
            CurrentInstructionText.text = "Waiting...";
            return;
        }
        else if (pInEvent == EventState.CONFIRMINGCALLTOMOBILIZE || pInEvent == EventState.CONFIRMINGRESOURCEPLANNING ||
            pInEvent == EventState.CONFIRMINGMOBILEHOSPITAL || pInEvent == EventState.CONFIRMINGFORECAST)
        {
            CurrentInstructionText.text = "Playing Event Card\nDo you confirm?";
            return;
        }
        else if (pInEvent == EventState.FORECAST)
        {
            CurrentInstructionText.text = "Event - Forecasting \nSelect any card.\nUse arrows to move";
            return;
        }
        else if (pInEvent == EventState.RESOURCEPLANNING)
        {
            CurrentInstructionText.text = "Event - Resource Planning \nSelect any card.\nUse arrows to move";
            return;
        }
        else if (pInEvent == EventState.CALLTOMOBILIZE)
        {
            if(movingPawn != null)
                CurrentInstructionText.text = "Event - Call to Mobilize \nMove 1-2 or accept to stay";
            else
                CurrentInstructionText.text = "Event - Call to Mobilize \nWaiting";
            
            return;
        }
        if (theGame.MobileHospitalInExecution && theGame.MobileHospitalPlayer == _player)
        {
            CurrentInstructionText.text = "Event - Mobile Hospital \nRemove a cube.";
            return;
        }

        if (game.CurrentPlayer != _player)
        {

            if(cardsState == CardGUIStates.CardsDiscarding)
            {
                CurrentInstructionText.text = "Discard a card";
                return;
            }
            else if (cardsState == CardGUIStates.CardsExpandedShareAction)
            {
                if(_player.PlayerCardsInHand.Contains(game.CurrentPlayer.GetCurrentCity()))
                    CurrentInstructionText.text = "Share your card?";
                else CurrentInstructionText.text = "Accept card?";
                return;
            }
            else
            {
                CurrentInstructionText.text = "Not your turn.";
                return;
            }
        }
        string textToreturn = PlayerModel.ActionsRemaining + " actions left."; 

        string additionalMessage = "";

        if(cardsState == CardGUIStates.CardsDiscarding)
        {
            if(selectedCards.Count == 0)
            {
                additionalMessage += "Select to discard";
            }
            else
            {
                if (selectedCards[0] > 23)
                    additionalMessage += "Use/Discard event";
                else
                    additionalMessage += "Discard city";
            }   
        }
        else if(ActionSelected == ActionTypes.Move)
        {
            additionalMessage += "Move your pawn";
        }
        else if(ActionSelected == ActionTypes.Fly)
        {
            if(cardsState == CardGUIStates.CardsExpandedFlyActionToSelect)
            {
                additionalMessage += "Pick a card to fly to";
            }
            else if(cardsState == CardGUIStates.CardsExpandedFlyActionSelected)
            {
                additionalMessage += "Complete action?";
            }
        }
        else if (ActionSelected == ActionTypes.Charter)
        {
            if (cardsState == CardGUIStates.CardsExpandedCharterActionToSelect)
            {
                additionalMessage += "Move to any city";
            }
        }
        else if (ActionSelected == ActionTypes.Treat)
        {
            additionalMessage += "Pick a cube";
        }
        else if (ActionSelected == ActionTypes.Share)
        {
            if (cardsState == CardGUIStates.CardsExpandedShareAction)
            {

                if (PlayerModel.PlayerCardsInHand.Contains(_player.GetCurrentCity()))
                    additionalMessage += "Share you card?";
                else
                    additionalMessage += "Wait for response...";
            }
        }
        else if (ActionSelected == ActionTypes.FindCure)
        {
            additionalMessage += "Complete action?";
        }
        else if (ActionSelected == ActionTypes.CharacterAction)
        {
            if (PlayerModel.Role == Player.Roles.Virologist)
                additionalMessage += "Remove a cube";
            else if (PlayerModel.Role == Player.Roles.Pilot)
            {
                if (pilotCitySelected == -1)
                    additionalMessage += "Touch city within 2";
                else
                {
                    if (pawnPilotSelected != null)
                        additionalMessage += "Bring pawn along?";
                    else
                    {
                        additionalMessage += "Complete move?";
                    }
                }
            }
        }

        if (additionalMessage != "")
        {
            textToreturn += "\n" + additionalMessage;
        }

        CurrentInstructionText.text = textToreturn;
    }

    public void drawLater(float time)
    {
        _isAnimating = true;
        this.ExecuteLater(time, doneAnimating);
    }

    void doneAnimating()
    {
        _isAnimating = false;
        draw();
    }


    internal void ChangeToInEvent(EventState state, bool shouldDraw = true)
    {
        PInEventCard = state;
        if (shouldDraw)
            draw();
        //else
        //    ClearContextButtons();
    }

}

public enum ActionTypes
{
    Move,
    Fly,
    Charter,
    Treat,
    Share,
    FindCure,
    CharacterAction,
    None
}

public enum CardGUIStates
{
    None,
    CardsExpanded,
    CardsExpandedFlyActionToSelect,
    CardsExpandedFlyActionSelected,
    CardsExpandedShareAction,
    CardsExpandedVirologistAction,
    CardsExpandedCharterActionToSelect,
    CardsExpandedCureActionToSelect,
    CardsExpandedCureActionSelected,
    CardsDiscarding
}

public enum ContextButtonStates
{
    Reject,
    Accept,
    None
}