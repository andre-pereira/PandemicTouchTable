using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ENUMS;
using static GameGUI;
using static Game;

public class EGameOver : EngineEvent
{
    private GameOverReasons tooManyOutbreaks;

    public EGameOver(GameOverReasons tooManyOutbreaks)
    {
        this.tooManyOutbreaks = tooManyOutbreaks;
        theGame.setCurrentGameState(GameState.GAME_OVER);
    }

    public override void Do(Timeline timeline)
    {
    }

}


