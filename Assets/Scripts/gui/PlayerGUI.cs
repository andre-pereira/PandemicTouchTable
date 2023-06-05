using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using System;
using UnityEditor;
using Unity.VisualScripting;
using System.ComponentModel;
using static ENUMS;

public class PlayerGUI : MonoBehaviour
{

    #region Properties and Fields
    private const string WaitForYourTurnText = "Wait for your turn.";
    GameGUI gui;
    Game game;

    bool _isAnimating = false;
    List<int> _drawnCards = new List<int>();
    private GameObject movingPawn = null;
    public CardGUIStates cardsState { get; private set; }
    private List<int> selectedCards;
    private GameObject flyLine;
    private List<PlayerGUI> playersToShareGUI;

    public int Position;

    public ActionTypes ActionSelected;

    Player _player = null;
    public TMPro.TextMeshProUGUI CurrentInstructionText;
    public TMPro.TextMeshProUGUI playerNameText;
    
    public RoleCardDisplay roleCard;
    private Image roleCardBackground;

    public GameObject PlayerCards;
    private List<GameObject> cardsInHand;

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

    #endregion

    public void init()
    {
        gui = GameGUI.gui;
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
        Color targeColor = roleCard.RoleCardData.roleColor;
        targeColor.a = GameGUI.gui.playerUIOpacity;
        GetComponent<Image>().color = targeColor;

        changeContextText();

        playersToShareGUI = new List<PlayerGUI>();

        playerNameText.text = PlayerModel.Name;
        selectedCards = new List<int>();
    }

    public void draw()
    {
        if (_isAnimating) return;
        if (PlayerModel == null) return;

        cardsInHand.Clear();
        PlayerCards.DestroyChildrenImmediate();

        HorizontalLayoutGroup layout = PlayerCards.GetComponent<HorizontalLayoutGroup>();

        foreach (int cardToAdd in _player.PlayerCardsInHand)
        {
            cardsInHand.Add(AddPlayerCardToTransform(cardToAdd, PlayerCards.transform, true));
        }

        if (_player.PlayerCardsInHand.Count > 6)
        {
            layout.gameObject.transform.localScale = new Vector3(0.86f, 0.86f, 1f);
            layout.padding.left = -695;
            layout.spacing = 31f;
            layout.childForceExpandWidth = false;
            layout.childControlWidth = false;
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
        else if (cardsState != CardGUIStates.None || PlayerModel != game.CurrentPlayer)
        {
            if (_player.PlayerCardsInHand.Count <= 6)
            {
                if(cardsState == CardGUIStates.CardsDiscarding)
                {
                    cardsState = CardGUIStates.None;
                }
                layout.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                layout.spacing = 30;
                layout.padding.left = -585;
                layout.childForceExpandWidth = false;
                layout.childControlWidth = false;
            }
        }
        else
        {
            layout.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            layout.spacing = 22.5f;
            layout.padding.left = -15;
            layout.childForceExpandWidth = true;
            layout.childControlWidth = true;
        }


        if(PlayerModel == game.CurrentPlayer)
        {
            bool moveAction = false;
            bool flyAction = false;
            bool charterAction = false;
            bool findCureAction = false;
            bool treatAction = false;
            bool shareAction = false;

            //changeContextText(true);
            if (PlayerModel.ActionsRemaining > 0)
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

                    if (PlayerModel.CurrentCityScript().cubesInCity())
                        treatAction = true;

                    if (!game.RedCure && PlayerModel.RedCardsInHand.Count > 3 || !game.YellowCure && PlayerModel.YellowCardsInHand.Count > 3 || !game.BlueCure && PlayerModel.BlueCardsInHand.Count > 3)
                        if(PlayerModel.GetCurrentCity() == game.InitialCityID)
                            findCureAction = true;

                    playersToShareGUI.Clear();

                    int countOtherPlayerInCity = 0;
                    foreach (Player player in PlayerModel.CurrentCityScript().PlayersInCity)
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
                        || cardsState == CardGUIStates.CardsExpandedCureActionToSelect || cardsState == CardGUIStates.CardsExpandedShareAction)
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
            //ClearSelectedAction();

        }
        else
        {
            disableAllActions();
            if (cardsState == CardGUIStates.CardsExpandedShareAction)
            {
                if (PlayerModel.PlayerCardsInHand.Contains(_player.GetCurrentCity()))
                    getCardInHand(PlayerModel.GetCurrentCity()).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
            }
        }

        changeContextText();
    }

    #region Buttons

    public void ContextButtonClicked(int buttonType)
    {
        if(buttonType == 2)
        {
            Timeline.theTimeline.addEvent(new PDiscardCard(selectedCards[0],this));
        }
        if (buttonType == 1)
        {
            if (cardsState == CardGUIStates.CardsExpandedFlyActionSelected)
            {
                Timeline.theTimeline.addEvent(new PFlyToCity(selectedCards[0]));
            }
            else if (cardsState == CardGUIStates.CardsExpandedCureActionSelected)
            {
                Timeline.theTimeline.addEvent(new PCureDisease(game.Cities[selectedCards[0]].city.virusInfo.virusName, selectedCards));
            }
            else if (cardsState == CardGUIStates.CardsExpandedShareAction)
            {
                if (this != GameGUI.currentPlayerPad())
                {
                    if (PlayerModel.PlayerCardsInHand.Contains(PlayerModel.GetCurrentCity()))
                        Timeline.theTimeline.addEvent(new PShareKnowledge(this, GameGUI.currentPlayerPad()));
                    else Timeline.theTimeline.addEvent(new PShareKnowledge(GameGUI.currentPlayerPad(),this));
                }
            }
        }

        if(buttonType == 0 && ActionSelected == ActionTypes.Share)
        {
            foreach (PlayerGUI playerGUI in playersToShareGUI)
            {
                playerGUI.ActionSelected = ActionTypes.None;
                playerGUI.ContextButtonClicked(0);
                
            }

        }

        if(this != GameGUI.currentPlayerPad())
        {
            GameGUI.currentPlayerPad().ClearSelectedAction();
            GameGUI.currentPlayerPad().draw();
        }

        cardsState = CardGUIStates.None;
        ClearSelectedAction();
        draw();
    }

    public void ActionButtonClicked(int action)
    {
        Debug.Log("Action button clicked: " + action);
        if (PlayerModel == null) return;
        if (PlayerModel != game.CurrentPlayer) return;
        if (PlayerModel.ActionsRemaining <= 0) return;

        ClearSelectedAction();

        City currentCity = game.Cities[PlayerModel.GetCurrentCity()];
        switch (action)
        {
            case 0: //move
                ActionSelected = ActionTypes.Move;
                MoveActionBackground.color = new Color(1f, 1f, 1f, .25f);
                CreateMovingPawn(currentCity);
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
                CreateMovingPawn(currentCity);
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
                    playerGUI.ContextButtons[0].SetActive(true);
                    playerGUI.ContextButtons[1].SetActive(true);
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
            case 6: //character action
                if (_player.Role == Player.Roles.Virologist || _player.Role == Player.Roles.Pilot)
                {
                    if (_player.Role == Player.Roles.Virologist)
                    {
                        cardsState = CardGUIStates.CardsExpandedVirologistAction;
                    }
                    ActionSelected = ActionTypes.CharacterAction;
                    roleCardBackground.GetComponent<Outline>().enabled = true;
                }
                break;
        }
        changeContextText();
    }

    private void CardInHandClicked(int cardClicked)
    {
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
                    if (selectedCards.Count == 4)
                        return;
                    if (selectedCards.Count == 0 || getCardInHand(selectedCards[0]).GetComponent<CityCardDisplay>().CityCardData.virusInfo.virusName == cardClickedScript.CityCardData.virusInfo.virusName)
                    {
                        selectedCards.Add(cardClicked);
                        getCardInHand(cardClicked).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
                        if (selectedCards.Count == 4)
                        {
                            cardsState = CardGUIStates.CardsExpandedCureActionSelected;
                            ContextButtons[1].SetActive(true);
                        }
                    }

                }
            }
        }


    }

    public void CityClicked(City city)
    {
        if (ActionSelected == ActionTypes.Treat && _player.GetCurrentCity() == city.city.cityID 
            && _player.ActionsRemaining > 0 && city.cubesInCity())
        {
            Timeline.theTimeline.addEvent(new PTreatDisease(city));
        }
    }

    internal void CubeClicked(City city, VirusName virusName)
    {
        if (ActionSelected == ActionTypes.Treat && _player.GetCurrentCity() == city.city.cityID 
            && _player.ActionsRemaining > 0 && city.cubesInCity())
        {
            Timeline.theTimeline.addEvent(new PTreatDisease(city, virusName));
        }
    }

    #endregion

    private void disableAllActions()
    {
        MoveAction.SetActive(false);
        FlyAction.SetActive(false);
        CharterAction.SetActive(false);
        TreatAction.SetActive(false);
        ShareAction.SetActive(false);
        FindCureAction.SetActive(false);
    }

    private void CreateMovingPawn(City currentCity)
    {
        currentCity.removePawn(game.CurrentPlayer);
        currentCity.draw();
        movingPawn = Instantiate(gui.PawnPrefab, currentCity.transform.position, currentCity.transform.rotation, gui.AnimationCanvas.transform);
        movingPawn.GetComponent<Image>().color = roleCard.RoleCardData.roleColor;
        movingPawn.GetComponent<Pawn>().canMove = true;
        movingPawn.GetComponent<Outline>().enabled = true;
    }

    public void ClearSelectedAction()
    {
        selectedCards.Clear();
        
        if(flyLine!=null)
        {
            Destroy(flyLine);
        }

        if (movingPawn != null)
        {
            Destroy(movingPawn);
            City currentCity = game.Cities[PlayerModel.GetCurrentCity()];
            currentCity.addPawn(game.CurrentPlayer);
            currentCity.draw();
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

        ContextButtons[0].SetActive(false);
        ContextButtons[1].SetActive(false);
        ContextButtons[2].SetActive(false);

        changeContextText();
    }

    public GameObject AddPlayerCardToTransform(int cardToAdd, Transform transform, bool withButtonComponent)
    {
        GameObject cardToAddObject;
        if (cardToAdd > 23)
        {
            cardToAddObject = Instantiate(gui.EventCardPrefab, transform);
            cardToAddObject.GetComponent<EventCardDisplay>().EventCardData = gui.Events[cardToAdd - 24];
            
        }
        else
        {
            cardToAddObject = Instantiate(gui.CityCardPrefab, transform);
            cardToAddObject.GetComponent<CityCardDisplay>().CityCardData = game.Cities[cardToAdd].city;

        }
        if (withButtonComponent)
        {
            var buttonComponent = cardToAddObject.AddComponent<Button>();
            buttonComponent.onClick.AddListener(() => CardInHandClicked(cardToAdd));
        }
        return cardToAddObject;
    }

    private void CreateLineBetweenCities(City cityToMoveTo, City cityToMoveFrom)
    {
        flyLine = new GameObject("Line - FlyAction");
        flyLine.transform.SetParent(gui.AnimationCanvas.transform, false);
        flyLine.transform.position = cityToMoveFrom.PawnsInCity[_player.Position].transform.position;
        flyLine.AddComponent<LineRenderer>();
        LineRenderer lr = flyLine.GetComponent<LineRenderer>();
        lr.sortingLayerName = "Animation";
        lr.material = gui.lineMaterial;
        lr.startColor = roleCard.RoleCardData.roleColor;
        lr.endColor = roleCard.RoleCardData.roleColor;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, cityToMoveFrom.PawnsInCity[_player.Position].transform.position);
        lr.SetPosition(1, cityToMoveTo.transform.position);
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
                additionalMessage += "Pick a card";
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
            additionalMessage += "Read role action text";
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