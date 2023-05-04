using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class Game : MonoBehaviour {

  public enum PlayerPrefSettings
  {
    LAST_FILE_LOADED
  }

  public enum PieceType
  {
    INVALID = -1,
    NUM_TYPES
  }

  public enum GameState
  {
    INVALID = -1,
    LOGIN,
    PLAY,
    GAME_OVER
  }
  #region GameOptions
    #endregion

  public static Game theGame = null;

  public Player CurrentPlayer = null;
  public GameState CurrentGameState = GameState.INVALID;

  public List<int> GiftDeck = null;
  public int NumCenterChips = -1;

  public void init()
  {
    // Reset any state here. When we undo, all the events are re-executed and the first event will
    // call this function to cleanup the old game state.
    GiftDeck = new List<int>();
    NumCenterChips = 0;
  }
  public void OnEnable()
  {
    theGame = this;
    //Profiler.maxNumberOfSamplesPerFrame = -1;
  }
  public void OnDestroy()
  {
    if (theGame == this) theGame = null;
  }
}
