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
        theGame.CurrentPlayer = PlayerList.nextPlayer(_player);
        theGame.CurrentPlayer.ResetTurn();
        theGame.setCurrentGameState(GameState.PLAYERACTIONS);
    }

    public override float Act(bool qUndo = false)
    {
        PlayerGUI nextPlayerGUI = GameGUI.currentPlayerPad();
        nextPlayerGUI.draw();
        _playerGui.draw();
        gui.draw();
        return 0;
    }

    
}