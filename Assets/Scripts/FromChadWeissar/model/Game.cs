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
        SETTINGBOARD,
        PLAYERACTIONS,
        DRAW1STPLAYERCARD,
        DRAW2NDPLAYERCARD,
        EPIDEMIC,
        DRAWINFECTCARDS,
        OUTBREAK,
        GAME_OVER
    }

    public enum EpidemicGameState
    {

        EPIDEMICINCREASE,
        EPIDEMICINFECT,
        EPIDEMICINTENSIFY
    }

    public int InfectionRate = 0;
    public int[] InfectionRateValues = new int[] { 2, 2, 3, 4 };
    private int numberOfDrawnInfectCards = 0;

    public int OutbreakCounter = 0;
    public List<int> OutbreakTracker = new List<int>();

    public int InitialCityID = 13;

    public static Game theGame = null;

    public Player CurrentPlayer = null;

    public GameState CurrentGameState { get; private set; } = GameState.INVALID;
    public GameState previousGameState { get; private set; } = GameState.INVALID;
    public EpidemicGameState epidemicGameState = EpidemicGameState.EPIDEMICINCREASE;

    private bool actionsInitiated = false;
    public bool actionCompleted = false;

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
    private bool turnEnded = false;

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
        if(CurrentPlayer.PlayerCardsInHand.Count < 7)
            Timeline.theTimeline.addEvent(new EDealCardToPlayer(CurrentPlayer, true));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            test();
        }
        
        foreach (Player player in PlayerList.getAllPlayers())
        {
            if(player.PlayerCardsInHand.Count > 6)
            {
                return;
            }
        }

        if (CurrentGameState == GameState.DRAW1STPLAYERCARD || CurrentGameState == GameState.DRAW2NDPLAYERCARD)
        {
            if (actionsInitiated == false)
            {
                actionsInitiated = true;
                Debug.Log("Draw Player Card: " + CurrentGameState);
                Timeline.theTimeline.addEvent(new EDealCardToPlayer(CurrentPlayer, true));
            }
            if (actionCompleted == true)
            {
                if (CurrentPlayer.PlayerCardsInHand.Count < 7)
                {
                    if (CurrentGameState == GameState.DRAW1STPLAYERCARD)
                        setCurrentGameState(GameState.DRAW2NDPLAYERCARD);
                    else setCurrentGameState(GameState.DRAWINFECTCARDS);
                }
            }
        }
        else if (CurrentGameState == GameState.EPIDEMIC)
        {
            if (epidemicGameState == EpidemicGameState.EPIDEMICINCREASE)
            {
                Timeline.theTimeline.addEvent(new EIncreaseInfectionRate());
                epidemicGameState = EpidemicGameState.EPIDEMICINFECT;
                Timeline.theTimeline.addEvent(new EDrawInfectionCard(3, true));
            }
            else if (epidemicGameState == EpidemicGameState.EPIDEMICINFECT)
            {
                if (actionCompleted == true)
                {
                    epidemicGameState = EpidemicGameState.EPIDEMICINTENSIFY;
                    Timeline.theTimeline.addEvent(new EIntensify());
                    if (previousGameState == GameState.DRAW1STPLAYERCARD)
                        setCurrentGameState(GameState.DRAW2NDPLAYERCARD);
                    else if (previousGameState == GameState.DRAW2NDPLAYERCARD)
                        setCurrentGameState(GameState.DRAWINFECTCARDS);
                }
            }
        }
        else if (CurrentGameState == GameState.OUTBREAK)
        {
            if (actionCompleted == true)
            {
                OutbreakTracker.Clear();
                setCurrentGameState(GameState.DRAWINFECTCARDS);
            }
        }
        else if (CurrentGameState == GameState.DRAWINFECTCARDS)
        {
            if (numberOfDrawnInfectCards < InfectionRateValues[InfectionRate])
            {
                if (actionsInitiated == false)
                {
                    actionsInitiated = true;
                    Timeline.theTimeline.addEvent(new EDrawInfectionCard(1, true));
                }
                if (actionCompleted == true)
                {
                    numberOfDrawnInfectCards++;
                    actionsInitiated = false;
                    actionCompleted = false;
                }
            }
            else
            {
                if (!turnEnded)
                {
                    Timeline.theTimeline.addEvent(new PEndTurn());
                    turnEnded = true;
                }
            }
        }
    }

    public void Awake()
    {
        theGame = this;
    }

    public void OnDestroy()
    {
        if (theGame == this) theGame = null;
    }

    internal void setCurrentGameState(GameState state)
    {
        turnEnded = false;
        numberOfDrawnInfectCards = 0;
        previousGameState = CurrentGameState;
        CurrentGameState = state;
        actionsInitiated = false;
        actionCompleted = false;
        if(state == GameState.EPIDEMIC)
            epidemicGameState = EpidemicGameState.EPIDEMICINCREASE;
    }
}
