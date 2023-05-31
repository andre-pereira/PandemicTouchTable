using UnityEngine;
using static Game;
using static GameGUI;

internal class EOutbreak : EngineEvent
{
    private City originOfOutbreak;

    public EOutbreak(City origin)
    {
        this.originOfOutbreak = origin;
    }

    public override void Do(Timeline timeline)
    {
        theGame.setCurrentGameState(GameState.OUTBREAK);
    }

    public override float Act(bool qUndo = false)
    {
        gui.BigTextMessage.text = "Outbreak!";
        return 0f;
    }
}