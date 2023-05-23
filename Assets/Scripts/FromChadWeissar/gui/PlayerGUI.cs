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

    public Player Player
    {
        get { if (_player == null) _player = PlayerList.playerAtPosition(Position); return _player; }
        set { _player = value; }
    }

    public int Position;

    public void init()
    {
        gui = GameGUI.theGameGUI;
        game = Game.theGame;
        _player = null;
        roleCard.RoleCardData = GameGUI.theGameGUI.roleCards[(int)Player.Role];
        Color targeColor = roleCard.RoleCardData.roleColor;
        targeColor.a = GameGUI.theGameGUI.playerUIOpacity;
        GetComponent<Image>().color = targeColor;

        if (Game.theGame.CurrentPlayer == Player)
        {
            CurrentInstructionText.text += "Your turn. You have " + "";
        }
        else
        {
            CurrentInstructionText.text += WaitForYourTurnText;
        }
        playerNameText.text = Player.Name;
    }

    bool _isAnimating = false;
    List<int> _drawnCards = new List<int>();

    public void draw()
    {
        if (_isAnimating) return;
        if (Player == null) return;

        PlayerCards.DestroyChildrenImmediate();

        foreach (int cardToAdd in _player.CardsInHand)
        {
            AddPlayerCardToTransform(cardToAdd, PlayerCards.transform);
        }

        if(Player == game.CurrentPlayer)
        {
            CurrentInstructionText.text = "Your turn. You have " + Player.ActionsRemaining + " actions remaining.";
            if (Player.ActionsRemaining > 0)
            {
                MoveAction.SetActive(true);


                FlyAction.SetActive(true);
                CharterAction.SetActive(true);

                TreatAction.SetActive(true);
                ShareAction.SetActive(true);

                
                FindCureAction.SetActive(true);
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