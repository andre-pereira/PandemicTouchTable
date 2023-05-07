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

    public static Game theGame = null;

    public Player CurrentPlayer = null;
    public GameState CurrentGameState = GameState.INVALID;

    public List<int> GiftDeck = null;
    public int NumCenterChips = -1;

    public void init()
    {
        // Reset any state here. When we undo, all the events are re-executed and the first event will
        // call this function to cleanup the old game state.

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
