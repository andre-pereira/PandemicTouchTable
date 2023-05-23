using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using System;
using UnityEditor;
using Unity.VisualScripting;

public class PlayerGUI : MonoBehaviour
{
    private const string WaitForYourTurnText = "Wait for your turn.";
    GameGUI gui;
    Game game;

    Player _player = null;
    public TMPro.TextMeshProUGUI CurrentInstructionText;
    public TMPro.TextMeshProUGUI playerNameText;
    public RoleCardDisplay roleCard;
    public GameObject PlayerCards;

    public GameObject MoveAction;
    public GameObject FlyAction;
    public GameObject CharterAction;
    public GameObject TreatAction;
    public GameObject ShareAction;
    public GameObject FindCureAction;

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
        _player = null;
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

        PlayerCards.DestroyChildrenImmediate();

        foreach (int cardToAdd in _player.CardsInHand)
        {
            AddPlayerCardToTransform(cardToAdd, PlayerCards.transform);
        }

        if(PlayerModel == game.CurrentPlayer)
        {
            CurrentInstructionText.text = "Your turn. You have " + PlayerModel.ActionsRemaining + " actions remaining.";
            if (PlayerModel.ActionsRemaining > 0)
            {
                MoveAction.SetActive(true);

                bool flyAction = false;
                bool charterAction = false;
                bool findCureAction = false;
                bool shareAction = false;
                List<int>[] cardsOfEachColor = new List<int>[3]; 
                foreach (int card in PlayerModel.CardsInHand)
                {
                    if (card < 24)
                    {
                        if(card != PlayerModel.CurrentCity)
                            flyAction = true;
                        else
                            charterAction = true;
                    }
                }
                FlyAction.SetActive(flyAction);
                CharterAction.SetActive(charterAction);
                FindCureAction.SetActive(findCureAction);

                bool treatAction = false;
                if(PlayerModel.CurrentCityScript().numberOfInfectionCubes > 0)
                    treatAction = true;
                TreatAction.SetActive(treatAction);

                if(!game.RedCure && PlayerModel.RedCardsInHand.Count>3 || !game.YellowCure && PlayerModel.YellowCardsInHand.Count > 3 || !game.BlueCure && PlayerModel.BlueCardsInHand.Count > 3)
                    FindCureAction.SetActive(treatAction);

                foreach (Player player in PlayerModel.CurrentCityScript().PawnsInCity)
                {
                    if(player != _player)
                    {
                        if(player.CardsInHand.Contains(_player.CurrentCity) || _player.CardsInHand.Contains(_player.CurrentCity))
                            shareAction = true;
                    }
                }
                ShareAction.SetActive(shareAction);
            }
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
        return cardToAddObject;
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