internal class EOutbreak : EngineEvent
{
    private City originOfOutbreak;

    public EOutbreak(City origin)
    {
        this.originOfOutbreak = origin;
    }

    public override void Do(Timeline timeline)
    {
        throw new System.NotImplementedException();
    }

    public override float Act(bool qUndo = false)
    {
        return 0f;
    }
}