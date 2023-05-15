using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EIncreaseInfectionRate : EngineEvent
{
    GameGUI gui = GameGUI.theGameGUI;
    Game game = Game.theGame;

    public override void Do(Timeline timeline)
    {
        game.InfectionRate++;
    }

    public override float Act(bool qUndo = false)
    {
        gui.InfectionRateMarker.transform.DOMove(gui.InfectionRateMarkerTransforms[game.InfectionRate].position, 1f);
        return 0f;
    }
}
