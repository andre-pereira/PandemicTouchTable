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
    public PlayerGUI playerGui;

    public List<int> PlayerCardsInHand { get; private set; }
    public List<int> CityCardsInHand { get; private set; }
    public List<int> RedCardsInHand { get; private set; }
    public List<int> YellowCardsInHand { get; private set; }
    public List<int> BlueCardsInHand { get; private set; }
    public List<int> EventCardsInHand { get; private set; }

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
    public string Name;
    private int currentCity;


    public Roles Role { get; set; }
    public int ActionsRemaining { get; internal set; }

    public Player(int tablePositionId, Roles playerRole, string playerName)
    {
        playerGui = GameGUI.playerPadForPosition(tablePositionId);
        Position = tablePositionId;
        Role = playerRole;
        Name = playerName;
        PlayerCardsInHand = new List<int>();
        CityCardsInHand = new List<int>();
        RedCardsInHand = new List<int>();
        YellowCardsInHand = new List<int>();
        BlueCardsInHand = new List<int>();
        EventCardsInHand = new List<int>();
    }

    public void AddCardToHand(int card)
    {
        PlayerCardsInHand.Add(card);
        if (card < 24)
        {
            CityCardsInHand.Add(card);
            CityCardsInHand.Sort();
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
        }
        else
            EventCardsInHand.Add(card);
        PlayerCardsInHand.Sort();
    }


    public City GetCurrentCityScript()
    {
        return Game.theGame.Cities[currentCity];
    }

    internal int GetCurrentCity()
    {
        return currentCity;
    }

    internal void UpdateCurrentCity(int cityID, bool updateRoles)
    {
        game.Cities[currentCity].removePawn(this);
        currentCity = cityID;
        game.Cities[cityID].addPawn(this);
        if (updateRoles)
        {
            if (Role == Roles.ContainmentSpecialist)
                Timeline.theTimeline.addEvent(new PContainSpecialistRemoveWhenEntering(cityID));
        }
    }

    internal void RemoveCardInHand(int cityID)
    {
        PlayerCardsInHand.Remove(cityID);
        if (cityID < 24) {
            CityCardsInHand.Remove(cityID);
            switch (game.Cities[cityID].city.virusInfo.virusName)
            {
                case ENUMS.VirusName.Red:
                    RedCardsInHand.Remove(cityID);
                    break;
                case ENUMS.VirusName.Yellow:
                    YellowCardsInHand.Remove(cityID);
                    break;
                case ENUMS.VirusName.Blue:
                    BlueCardsInHand.Remove(cityID);
                    break;
                default:
                    break;
            }
        }
        else
            EventCardsInHand.Remove(cityID);
    }

    internal void ResetTurn()
    {
        ActionsRemaining = 4;
        playerGui.ClearSelectedAction();
    }
}
