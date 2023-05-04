using UnityEngine;

[System.Serializable]
public class EAddPlayer : EngineEvent
{
  public int TablePositionId;
  public Player.PlayerColors PlayerColor;

  EAddPlayer()
  {
    QUndoable = false;
  }
  public EAddPlayer( int tableId, Player.PlayerColors color )
  {
    TablePositionId = tableId;
    PlayerColor = color;
  }

  public override void Do(Timeline c)
  {
    //Debug.Log("EAddPlayer::Do position="+myTablePositionId);
    Player p = new Player() { Position = TablePositionId, Color = PlayerColor };
    PlayerList.Players.Add(p);
  }
}