using UnityEngine;

public class EEpidemic : EngineEvent
{
    public EEpidemic()
    {

    }

    public override void Do(Timeline timeline)
    {
        Timeline.theTimeline.addEvent(new EIncreaseInfectionRate());
        Timeline.theTimeline.addEvent(new EFlipCardAddCubes(3, false));

    }

    public override float Act(bool qUndo = false)
    {
        return 0f;
    }
}