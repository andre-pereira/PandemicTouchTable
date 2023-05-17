using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ENUMS;

public class EGameOver : EngineEvent
{
    private GameOverReasons tooManyOutbreaks;

    public EGameOver(GameOverReasons tooManyOutbreaks)
    {
        this.tooManyOutbreaks = tooManyOutbreaks;
    }

    public override void Do(Timeline timeline)
    {
        throw new System.NotImplementedException();
    }

}


