using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class Game : MonoBehaviour
{

    public enum PlayerPrefSettings
    {
        LAST_FILE_LOADED
    }


    public enum GameState
    {
        INVALID = -1,
        LOGIN,
        PLAY,
        GAME_OVER
    }

    public int InfectionRate = 0;
    public int[] InfectionRateValues = new int[] { 2, 2, 3, 4 };

    public int OutbreakCounter = 0;

    public int InitialCityID = 13;

    public static Game theGame = null;

    public Player CurrentPlayer = null;
    public GameState CurrentGameState = GameState.INVALID;

    public List<int> PlayerCards = null;
    public List<int> PlayerCardsDiscard = null;

    public List<int> InfectionCards = null;
    public List<int> InfectionCardsDiscard = null;

    public int RedCubes = 16;
    public int YellowCubes = 16;
    public int BlueCubes = 16;

    public bool RedCure = false;
    public bool YellowCure = false;
    public bool BlueCure = false;

    public City[] Cities { get; internal set; }

    public void init()
    {
        // Reset any state here. When we undo, all the events are re-executed and the first event will
        // call this function to cleanup the old game state.
        PlayerCards = new List<int>();
        PlayerCardsDiscard = new List<int>();
        InfectionCards = new List<int>();
        InfectionCardsDiscard = new List<int>();

        InfectionRate = 0;
        OutbreakCounter = 0;
    }

    public int GetCurrentInfectionRate()
    {
        return InfectionRateValues[InfectionRate];
    }

    public void test()
    {
        GameGUI.gui.draw();
    }

    public void Awake()
    {
        theGame = this;
    }

    public void OnDestroy()
    {
        if (theGame == this) theGame = null;
    }

}
