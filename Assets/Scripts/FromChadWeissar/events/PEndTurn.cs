using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEndTurn : PlayerEvent
{
    public PEndTurn() : base(Game.theGame.CurrentPlayer)
    {
    }

    public override void Do(Timeline timeline)
    {
        Game.theGame.CurrentPlayer = PlayerList.nextPlayer(_player);
        Game.theGame.CurrentPlayer.ActionsRemaining = 4;
    }

    public override float Act(bool qUndo = false)
    {
        PlayerGUI nextPlayerGUI = GameGUI.currentPlayerPad();
        nextPlayerGUI.draw();
        _playerGui.draw();
        return 0;
    }

    
}