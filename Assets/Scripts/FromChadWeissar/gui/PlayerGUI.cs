using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using System;
using UnityEditor;

public class PlayerGUI : MonoBehaviour
{
    GameGUI gui;
    Game game;

    Player _player = null;
    public TMPro.TextMeshProUGUI CurrentInstructionText;
    public TMPro.TextMeshProUGUI playerNameText;
    public RoleCardDisplay roleCard;
    public GameObject PlayerCards;

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
            CurrentInstructionText.text += "It is currently not your turn";
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
            cardToAddObject.GetComponent<CityCardDisplay>().CityCardData = gui.Cities[cardToAdd].GetComponent<City>().city;
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