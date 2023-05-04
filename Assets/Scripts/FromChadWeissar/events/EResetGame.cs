using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class EResetGame : EngineEvent
{
  public EResetGame()
  {
    QUndoable = false;
  }

  public override void Do(Timeline c)
  {
    Game.theGame.init();
    Game.theGame.CurrentPlayer = null;
    PlayerList.Players.Clear();
    foreach (PlayerGUI pad in GameGUI.theGameGUI.PlayerPads)
      pad.Player = null;
  }
}
