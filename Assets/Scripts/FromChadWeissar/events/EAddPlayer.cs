using UnityEngine;

[System.Serializable]
public class EAddPlayer : EngineEvent
{
  public int TablePositionId;
  public Player.Roles PlayerRole;

  EAddPlayer()
  {
    QUndoable = false;
  }
  public EAddPlayer( int tableId, Player.Roles role )
  {
    TablePositionId = tableId;
    PlayerRole = role;
  }

  public override void Do(Timeline c)
  {
    //Debug.Log("EAddPlayer::Do position="+myTablePositionId);
    Player p = new Player() { Position = TablePositionId, Role = PlayerRole };
    PlayerList.Players.Add(p);
  }
}