public class PTreatDisease : PlayerEvent
{
    private City city;

    public PTreatDisease(City city): base(Game.theGame.CurrentPlayer)
    {
        this.city = city;
    }

    public override void Do(Timeline timeline)
    {
        city.numberOfInfectionCubes -= 1;
        _player.ActionsRemaining -= 1;
        if (game.CurrentPlayer.ActionsRemaining == 0)
        {
            Timeline.theTimeline.addEvent(new PEndTurn());
        }
    }

    public override float Act(bool qUndo = false)
    {
        city.draw();
        _playerGui.draw();
        return 0;
    }
}