using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameGUI;
using static Game;

public class EIncreaseInfectionRate : EngineEvent
{
    private float ANIMATIONDURATION = 1f / gui.AnimationTimingMultiplier;

    public override void Do(Timeline timeline)
    {
        theGame.InfectionRate++;
    }

    public override float Act(bool qUndo = false)
    {
        GameObject moveFrom = gui.getInfectionRateMarker(theGame.InfectionRate - 1);
        GameObject moveTo = gui.getInfectionRateMarker(theGame.InfectionRate);
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
