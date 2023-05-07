using UnityEngine;

[System.Serializable]
public class EAddPlayer : EngineEvent
{
    public int TablePositionId;
    public Player.Roles PlayerRole;
    public string PlayerName;

    EAddPlayer()
    {
        QUndoable = false;
    }
    public EAddPlayer(int tableId, Player.Roles role, string playerName)
    {
        TablePositionId = tableId;
        PlayerRole = role;
        PlayerName = playerName;
    }

    public override void Do(Timeline c)
    {
        //Debug.Log("EAddPlayer::Do position="+myTablePositionId);
        Player p = new Player() { Position = TablePositionId, Role = PlayerRole, Name = PlayerName};
        PlayerList.Players.Add(p);
    }
}