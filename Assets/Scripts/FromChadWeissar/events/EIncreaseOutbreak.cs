using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EIncreaseOutbreak : EngineEvent
{
    GameGUI gui = GameGUI.theGameGUI;
    Game game = Game.theGame;

    public override void Do(Timeline timeline)
    {
        game.OutbreakCounter++;
        if(game.OutbreakCounter == 4)
        {
            timeline.addEvent(new EGameOver());
        }
        Debug.Log("DoEIncreaseOutbreak");
    }

    public override float Act(bool qUndo = false)
    {
        gui.OutbreakMarker.transform.DOMove(gui.OutbreakMarkerTransforms[game.OutbreakCounter].position, 1f);
        Debug.Log("ActEIncreaseOutbreak");
        return 0f;
    }


}
