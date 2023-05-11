using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EIncreaseOutbreak : EngineEvent
{
    GameGUI gui = GameGUI.theGameGUI;
    public override void Do(Timeline timeline)
    {
        Game.theGame.OutbreakCounter++;
        if(Game.theGame.OutbreakCounter == 4)
        {
            timeline.addEvent(new EGameOver());
        }
    }

    public override float Act(bool qUndo = false)
    {
        gui.OutbreakMarker.transform.DOMove(gui.OutbreakMarkerTransforms[Game.theGame.OutbreakCounter-1].position, 1f);

        return 0f;
    }


}
