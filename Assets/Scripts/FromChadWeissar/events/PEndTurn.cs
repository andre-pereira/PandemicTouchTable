using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game;

public class PEndTurn : PlayerEvent
{
    public PEndTurn() : base(Game.theGame.CurrentPlayer)
    {
    }

    public override void Do(Timeline timeline)
    {
        Game.theGame.CurrentPlayer = PlayerList.nextPlayer(_player);
        Game.theGame.CurrentPlayer.ResetTurn(); 
    }

    public override float Act(bool qUndo = false)
    {
        PlayerGUI nextPlayerGUI = GameGUI.currentPlayerPad();
        nextPlayerGUI.draw();
        _playerGui.draw();
        return 0;
    }

    
}