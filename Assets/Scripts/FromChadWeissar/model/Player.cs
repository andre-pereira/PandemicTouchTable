using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using JetBrains.Annotations;

public class Player
{
    public Game game = Game.theGame;
    public GameGUI gui = GameGUI.gui;

    public enum Roles
    {
        [Description("Containment Specialist")]
        ContainmentSpecialist,

        [Description("Pilot")]
        Pilot,

        [Description("Quarantine Specialist")]
        QuarantineSpecialist,

        [Description("Virologist")]
        Virologist
    };

    public int Position;
    public int Place;
    public string Name;
    private int currentCity;
    

    public Roles Role { get; set; }
    public int ActionsRemaining { get; internal set; }

    public Player()
    {
        CardsInHand = new List<int>();
        RedCardsInHand = new List<int>();
        YellowCardsInHand = new List<int>();
        BlueCardsInHand = new List<int>();
        EventCardsInHand = new List<int>();
    }

    public void AddCardToHand(int card)
    {
        CardsInHand.Add(card);
        if(card <24)
            switch (game.Cities[card].city.virusInfo.virusName)
            {
                case ENUMS.VirusName.Red:
                    RedCardsInHand.Add(card);
                    break;
                case ENUMS.VirusName.Yellow:
                    YellowCardsInHand.Add(card);
                    break;
                case ENUMS.VirusName.Blue:
                    BlueCardsInHand.Add(card);
                    break;
                default:
                    break;
            }
        else
            EventCardsInHand.Add(card);
        CardsInHand.Sort();
    }

    public List<int> CardsInHand;
    public List<int> RedCardsInHand;
    public List<int> YellowCardsInHand;
    public List<int> BlueCardsInHand;
    public List<int> EventCardsInHand;

    public City CurrentCityScript()
    {
        return Game.theGame.Cities[currentCity];
    }

    internal int GetCurrentCity()
    {
        return currentCity;
    }

    internal void UpdateCurrentCity(int cityID)
    {
        currentCity = cityID;
        game.Cities[cityID].addPawn(this);
    }


    #region State
    #endregion



}
