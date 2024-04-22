using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ENUMS;
using static GameGUI;
using static Game;
using Unity.VisualScripting;

public class EGameOver : EngineEvent
{
    public GameOverReasons Reason;

    public GameObject GameEndWin;
    public GameObject GameEndLose;

    public EGameOver(GameOverReasons reason)
    {
        this.Reason = reason;
        theGame.setCurrentGameState(GameState.GAME_OVER);
    }

    public override void Do(Timeline timeline)
    {


    }
    public override float Act(bool qUndo = false)
    {
        //GameObject gameOverPanel = Object.Instantiate(gui.GameEnd.GetComponent<Canvas>().transform.GetChild(0).gameObject, gui.AnimationCanvas.transform);

        //gameOverPanel.SetActive(true);
        if (Reason == GameOverReasons.PlayersWon)
        {
            gui.GameEndWin.SetActive(true);
        }
        else
        {
            gui.GameEndLose.SetActive(true);
        }
        return 0;
    }

}


