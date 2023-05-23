using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EIncreaseInfectionRate : EngineEvent
{
    private const float ANIMATIONDURATION = 1f;
    GameGUI gui = GameGUI.gui;
    Game game = Game.theGame;

    public override void Do(Timeline timeline)
    {
        game.InfectionRate++;
    }

    public override float Act(bool qUndo = false)
    {
        GameObject moveFrom = gui.getInfectionRateMarker(game.InfectionRate - 1);
        GameObject moveTo = gui.getInfectionRateMarker(game.InfectionRate);
        moveFrom.DestroyChildrenImmediate();
        GameObject marker = Object.Instantiate(gui.InfectionRateMarkerPrefab, moveFrom.transform.position, moveFrom.transform.rotation, gui.AnimationCanvas.transform);
        marker.transform.DOMove(moveTo.transform.position, ANIMATIONDURATION).OnComplete(() =>
        {
            Object.Destroy(marker);
            gui.drawBoard();
        });
        return ANIMATIONDURATION;
    }
}
