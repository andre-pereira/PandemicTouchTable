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

    public int InfectionRate = -1;
    public int[] InfectionRateValues = new int[] { 2, 2, 3, 4 };

    public int OutbreakCounter = -1;

    public int InitialCityID = 13;

    public static Game theGame = null;

    public Player CurrentPlayer = null;
    public GameState CurrentGameState = GameState.INVALID;

    public List<int> PlayerCards = null;


    public void init()
    {
        // Reset any state here. When we undo, all the events are re-executed and the first event will
        // call this function to cleanup the old game state.
        PlayerCards = new List<int>();
        InfectionRate = -1;
        OutbreakCounter = -1;
        Timeline.theTimeline.addEvent(new EIncreaseOutbreak());
        Timeline.theTimeline.addEvent(new EIncreaseInfectionRate());

    }

    public int GetCurrentInfectionRate()
    {
        return InfectionRateValues[InfectionRate];
    }

    public void test()
    {
        Timeline.theTimeline.addEvent(new EIncreaseOutbreak());
    }

    public void OnEnable()
    {
        theGame = this;
    }

    public void OnDestroy()
    {
        if (theGame == this) theGame = null;
    }
}
