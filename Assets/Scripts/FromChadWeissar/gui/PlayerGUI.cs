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


public class PlayerGUI : MonoBehaviour
{
    private const string WaitForYourTurnText = "Wait for your turn.";
    GameGUI gui;
    Game game;

    public ActionTypes ActionSelected;
    public Image ContextButton;

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

        if (Game.theGame.CurrentPlayer == PlayerModel)
        {
            CurrentInstructionText.text += "Your turn. You have " + "";
        }
        else
        {
            CurrentInstructionText.text += WaitForYourTurnText;
        }
        playerNameText.text = PlayerModel.Name;
    }

    bool _isAnimating = false;
    List<int> _drawnCards = new List<int>();


    public void draw()
    {
        if (_isAnimating) return;
        if (PlayerModel == null) return;

        bool cardsExpanded = false;
        if ((int)gui._state >= 1)
            cardsExpanded = true;

        cardsInHand.Clear();
        PlayerCards.DestroyChildrenImmediate();

        HorizontalLayoutGroup layout = PlayerCards.GetComponent<HorizontalLayoutGroup>();
        if (cardsExpanded)
        {
            layout.spacing = 130;
            layout.padding.left = -530;
        }
        else
        {
            layout.spacing = 23.6f;
            layout.padding.left = 0;
        }

        foreach (int cardToAdd in _player.CardsInHand)
        {
            cardsInHand.Add(AddPlayerCardToTransform(cardToAdd, PlayerCards.transform));
        }

        if(PlayerModel == game.CurrentPlayer)
        {
            bool moveAction = false;
            bool flyAction = false;
            bool charterAction = false;
            bool findCureAction = false;
            bool treatAction = false;
            bool shareAction = false;

            CurrentInstructionText.text = PlayerModel.ActionsRemaining + " actions left.";
            if (PlayerModel.ActionsRemaining > 0)
            {
                if (!cardsExpanded)
                {
                    CurrentInstructionText.text = PlayerModel.ActionsRemaining + " actions left.";
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
                        FindCureAction.SetActive(treatAction);

                    foreach (Player player in PlayerModel.CurrentCityScript().PawnsInCity)
                    {
                        if (player != _player)
                        {
                            if (player.CardsInHand.Contains(_player.GetCurrentCity()) || _player.CardsInHand.Contains(_player.GetCurrentCity()))
                                shareAction = true;
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
        else
        {
            CurrentInstructionText.text = WaitForYourTurnText;
            disableAllActions();
        }

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

        MoveActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        FlyActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        CharterActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        TreatActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        ShareActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        FindCureActionBackground.color = new Color(.2f, .2f, .2f, .2f);
        roleCardBackground.GetComponent<Outline>().enabled = false;

        CurrentInstructionText.text = PlayerModel.ActionsRemaining + " actions left.";

        switch (action)
        {
            case 0: //move
                ActionSelected = ActionTypes.Move;
                MoveActionBackground.color = new Color(1f, 1f, 1f, .25f);
                CurrentInstructionText.text = "Move your pawn";
                break;
            case 1: //fly
                ActionSelected = ActionTypes.Fly;
                FlyActionBackground.color = new Color(1f, 1f, 1f, .25f);

                CurrentInstructionText.text = "Discard [city], fly to [city]";
                break;
            case 2: //charter
                ActionSelected = ActionTypes.Charter;
                CharterActionBackground.color = new Color(1f, 1f, 1f, .25f);
                CurrentInstructionText.text = "Discard [current city], fly to any city";
                break;
            case 3: //treat
                ActionSelected = ActionTypes.Treat;
                TreatActionBackground.color = new Color(1f, 1f, 1f, .25f);
                CurrentInstructionText.text = "Remove 1 cube from city";
                break;
            case 4: //share
                ActionSelected = ActionTypes.Share;
                ShareActionBackground.color = new Color(1f, 1f, 1f, .25f);
                CurrentInstructionText.text = "Exchange [current city] with another player";
                break;
            case 5: //find cure
                ActionSelected = ActionTypes.FindCure;
                FindCureActionBackground.color = new Color(1f, 1f, 1f, .25f);
                CurrentInstructionText.text = "Discard 4 of same color, cure disease";
                break;
            case 6: //character action
                if (_player.Role == Player.Roles.Virologist || _player.Role == Player.Roles.Pilot)
                {
                    ActionSelected = ActionTypes.CharacterAction;
                    roleCardBackground.GetComponent<Outline>().enabled = true;
                    CurrentInstructionText.text = "Perform your character action";
                }
                break;
        }
    }


    public GameObject AddPlayerCardToTransform(int cardToAdd, Transform transform)
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
        var buttonComponent = cardToAddObject.AddComponent<Button>();
        buttonComponent.onClick.AddListener(() => HandleButton(cardToAdd));
        return cardToAddObject;
    }

    private void HandleButton(int cardToAdd)
    {
        Debug.Log("Card:" + cardToAdd + " Button Clicked. Player:" + _player.Role.ToString());
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