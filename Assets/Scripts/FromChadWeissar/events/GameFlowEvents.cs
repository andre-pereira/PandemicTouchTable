using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

// Game flow:
// EInitialize
// EStartRound
// EEndGame
public class EStartRound : EngineEvent
{
  public override void Do(Timeline timeline)
  {
    Game.theGame.CurrentPlayer = PlayerList.Players[0];
    Game.theGame.CurrentGameState = Game.GameState.PLAY;
  }
  public override float Act(bool qUndo = false)
  {
    // clear the load overlay
    //GameGUI.theGameGUI.LoadOverlay.SetActive(false);
    //GameGUI.theGameGUI.GameCanvas.SetActive(true);



    return 0;
  }
}

public class EEndGame : EngineEvent
{
  public override void Do(Timeline timeline)
  {
    Game.theGame.CurrentGameState = Game.GameState.GAME_OVER;
  }
  public override float Act(bool qUndo = false)
  {
    WindowsVoice.speak("Game Over.");
    GameGUI.theGameGUI.draw();
    return 0;
  }
}