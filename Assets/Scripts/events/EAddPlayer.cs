using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static GameGUI;
using static Game;

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
    public EAddPlayer(int tableId, Player.Roles role, string name)
    {
        TablePositionId = tableId;
        PlayerRole = role;
        PlayerName = name;
    }

    public override void Do(Timeline c)
    {
        Player p = new Player() { Position = TablePositionId, Role = PlayerRole, Name = PlayerName };
        PlayerList.Players.Add(p);
        p.UpdateCurrentCity(theGame.InitialCityID);
    }

    public override float Act(bool qUndo = false)
    {

        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(currentPawnImage.DOFade(0f, 0f));
        //sequence.Append(currentPawnImage.DOFade(1f, 1f));
        //sequence.Append(currentPawn.transform.DOMove(pawnPosition,1f));
        //sequence.Play();
        theGame.Cities[theGame.InitialCityID].draw();
        return 0f;
    }

}