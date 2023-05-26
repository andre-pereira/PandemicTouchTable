﻿using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using System;
using UnityEditor;
using Unity.VisualScripting;
using System.ComponentModel;


public class PlayerGUI : MonoBehaviour
{
    private const string WaitForYourTurnText = "Wait for your turn.";
    GameGUI gui;
    Game game;

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

    public int Position;

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
        
        playerNameText.text = PlayerModel.Name;
        selectedCards = new List<int>();
    }

    bool _isAnimating = false;
    List<int> _drawnCards = new List<int>();
    private GameObject movingPawn = null;
    private CardGUIStates cardsState;
    private List<int> selectedCards;
    private GameObject flyLine;

    public void draw()
    {
        if (_isAnimating) return;
        if (PlayerModel == null) return;

        cardsInHand.Clear();
        PlayerCards.DestroyChildrenImmediate();

        HorizontalLayoutGroup layout = PlayerCards.GetComponent<HorizontalLayoutGroup>();
        if (cardsState != CardGUIStates.None)
        {
            layout.spacing = 30;
            layout.padding.left = -585;
            layout.childForceExpandWidth = false;
            layout.childControlWidth = false;
        }
        else
        {
            layout.spacing = 23.6f;
            layout.padding.left = 0;
            layout.childForceExpandWidth = true;
            layout.childControlWidth = true;
        }

        foreach (int cardToAdd in _player.CardsInHand)
        {
            cardsInHand.Add(AddPlayerCardToTransform(cardToAdd, PlayerCards.transform, true));
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
                    foreach (int card in PlayerModel.CardsInHand)
                    {
                        if (card < 24)
                        {
                            if (card != PlayerModel.GetCurrentCity())
                                flyAction = true;
                            else
                                charterAction = true;
                        }
                    }

                    if (PlayerModel.CurrentCityScript().numberOfInfectionCubes > 0)
                        treatAction = true;

                    if (!game.RedCure && PlayerModel.RedCardsInHand.Count > 3 || !game.YellowCure && PlayerModel.YellowCardsInHand.Count > 3 || !game.BlueCure && PlayerModel.BlueCardsInHand.Count > 3)
                        findCureAction = true;

                    foreach (Player player in PlayerModel.CurrentCityScript().PlayersInCity)
                    {
                        if (player != _player)
                        {
                            if (player.CardsInHand.Contains(_player.GetCurrentCity()) || _player.CardsInHand.Contains(_player.GetCurrentCity()))
                                shareAction = true;
                        }
                    }
                }
                else
                { 
                    if(cardsState == CardGUIStates.CardsExpandedFlyActionToSelect || cardsState == CardGUIStates.CardsExpandedCharterActionToSelect)
                    {
                        ContextButtons[0].SetActive(true);
                        if(cardsState == CardGUIStates.CardsExpandedCharterActionToSelect)
                        {
                            getCardInHand(PlayerModel.GetCurrentCity()).GetComponent<CityCardDisplay>().border.gameObject.SetActive(true);
                        }
                    }
                    if (cardsState == CardGUIStates.CardsExpandedFlyActionSelected || cardsState == CardGUIStates.CardsExpandedCharterActionSelected)
                    {
                        ContextButtons[1].SetActive(true);
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
        }

        changeContextText();
    }

    public void ContextButtonClicked(int buttonType)
    {
        if (buttonType == 1)
        {
            if (cardsState == CardGUIStates.CardsExpandedFlyActionSelected)
            {
                Timeline.theTimeline.addEvent(new PFlyToCity(selectedCards[0]));
            }
        }

        cardsState = CardGUIStates.None;
        ClearSelectedAction();
        draw();
        ContextButtons[1].SetActive(false);
        ContextButtons[0].SetActive(false);
    }


    private void disableAllActions()
    {
        MoveAction.SetActive(false);
        FlyAction.SetActive(false);
        CharterAction.SetActive(false);
        TreatAction.SetActive(false);
        ShareAction.SetActive(false);
        FindCureAction.SetActive(false);
    }



    public void actionButtonClicked(int action)
    {

        if (PlayerModel == null) return;
        if (PlayerModel != game.CurrentPlayer) return;
        if (PlayerModel.ActionsRemaining <= 0) return;

        ClearSelectedAction();

        switch (action)
        {
            case 0: //move
                City currentCity = game.Cities[PlayerModel.GetCurrentCity()];
                ActionSelected = ActionTypes.Move;
                MoveActionBackground.color = new Color(1f, 1f, 1f, .25f);
                currentCity.removePawn(game.CurrentPlayer);
                currentCity.draw();
                movingPawn = Instantiate(gui.PawnPrefab, currentCity.transform.position, currentCity.transform.rotation, gui.AnimationCanvas.transform);
                movingPawn.GetComponent<Image>().color = roleCard.RoleCardData.roleColor;
                movingPawn.GetComponent<Pawn>().canMove = true;
                movingPawn.GetComponent<Outline>().enabled = true;
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
                break;
            case 5: //find cure
                ActionSelected = ActionTypes.FindCure;
                FindCureActionBackground.color = new Color(1f, 1f, 1f, .25f);
                cardsState = CardGUIStates.CardsExpandedCureAction;
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
            buttonComponent.onClick.AddListener(() => CardButtonClick(cardToAdd));
        }
        return cardToAddObject;
    }


    private void CardButtonClick(int cardClicked)
    {
        if (cardClicked < 24)
        {
            if (cardsState == CardGUIStates.CardsExpandedCharterActionToSelect)
            {
                CityCardDisplay cityCard = getCardInHand(cardClicked).GetComponent<CityCardDisplay>();
                if(cardClicked != _player.GetCurrentCity())
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
                    cityCard.border.gameObject.SetActive(true);
                }
            }

            if(cardsState == CardGUIStates.CardsExpandedCharterActionSelected)
            {

            }

            if (cardsState == CardGUIStates.CardsExpandedFlyActionToSelect || cardsState == CardGUIStates.CardsExpandedFlyActionSelected)
            {
                cardsState = CardGUIStates.CardsExpandedFlyActionSelected;
                selectedCards.Clear();
                selectedCards.Add(cardClicked);

                CityCardDisplay cityCard = getCardInHand(cardClicked).GetComponent<CityCardDisplay>();

                if (cityCard != null)
                {
                    if (flyLine != null)
                    {
                        Destroy(flyLine);
                    }

                    removeBorderFromCardsInHand();
                    cityCard.border.gameObject.SetActive(true);
                    City cityToMoveTo = game.Cities[cardClicked];
                    City cityToMoveFrom = game.Cities[_player.GetCurrentCity()];

                    cityToMoveFrom.PawnsInCity[_player.Position].gameObject.GetComponent<Outline>().enabled = true;

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
                    ContextButtons[1].SetActive(true);
                }
            }
        }

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

    public GameObject getCardInHand(int cityID)
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CityCardDisplay cityCard = cardsInHand[i].GetComponent<CityCardDisplay>();
            if (cityCard != null)
            {
                if (cityCard.CityCardData.cityID == cityID)
                {
                    return cardsInHand[i];
                }
            }
        }   
        return null;
    }

    private void changeContextText()
    {
        if (game.CurrentPlayer != _player)
        {
            CurrentInstructionText.text = "Not your turn.";
            return;
        }
        string textToreturn = PlayerModel.ActionsRemaining + " actions left."; 

        string additionalMessage = "";

        if(ActionSelected == ActionTypes.Move)
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
                additionalMessage += "Pick a city";
            }
            else if (cardsState == CardGUIStates.CardsExpandedCharterActionSelected)
            {
                additionalMessage += "Complete action?";
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
                additionalMessage += "Share card?";
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

    public void CityClicked()
    {
        Debug.Log("I am player " + Position);
        Debug.Log("City clicked");
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
    CardsExpandedCureAction,
    CardsExpandedVirologistAction,
    CardsExpandedCharterActionToSelect,
    CardsExpandedCharterActionSelected
}

public enum ContextButtonStates
{
    Reject,
    Accept,
    None
}