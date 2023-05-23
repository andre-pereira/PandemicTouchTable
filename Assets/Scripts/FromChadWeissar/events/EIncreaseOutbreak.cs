using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static ENUMS;

public class EIncreaseOutbreak : EngineEvent
{
    private const float ANIMATIONDURATION = 1f;
    GameGUI gui = GameGUI.gui;
    Game game = Game.theGame;

    public override void Do(Timeline timeline)
    {
        game.OutbreakCounter++;
        if(game.OutbreakCounter == 4)
        {
            timeline.addEvent(new EGameOver(GameOverReasons.TooManyOutbreaks));
        }
    }

    public override float Act(bool qUndo = false)
    {
        Transform moveFrom = gui.OutbreakMarkerTransforms[game.OutbreakCounter - 1];
        Transform moveTo = gui.OutbreakMarkerTransforms[game.OutbreakCounter];
        moveFrom.gameObject.DestroyChildrenImmediate();
        GameObject marker = Object.Instantiate(gui.OutbreakMarkerPrefab, moveFrom.position, moveFrom.rotation, gui.AnimationCanvas.transform);
        marker.transform.DOMove(moveTo.transform.position, ANIMATIONDURATION).OnComplete(() => 
        {
            Object.Destroy(marker);
            gui.drawBoard();
        });
        
        return ANIMATIONDURATION;
    }


}
