using static Game;

public class PForecast : PlayerEvent
{
    public PForecast() : base(theGame.CurrentPlayer)
    {
    }

    public override void Do(Timeline timeline)
    {
        //throw new System.NotImplementedException();
    }

    public override float Act(bool qUndo = false)
    {
        return base.Act(qUndo);
    }
}