using static Game;

public class PCallToMobilize : PlayerEvent
{
    public PCallToMobilize() : base(theGame.CurrentPlayer)
    {
    }

    public override void Do(Timeline timeline)
    {
        throw new System.NotImplementedException();
    }

    public override float Act(bool qUndo = false)
    {
        return base.Act(qUndo);
    }
}