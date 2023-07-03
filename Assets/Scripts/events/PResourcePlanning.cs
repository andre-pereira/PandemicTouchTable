using static Game;

public class PResourcePlanning : PlayerEvent
{
    public PResourcePlanning() : base(theGame.CurrentPlayer)
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