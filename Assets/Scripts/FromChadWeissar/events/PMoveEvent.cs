internal class PMoveEvent : PlayerEvent
{
    private int newCityID;
    private int oldCityID;
    private int numberOfActionsSpent;

    public PMoveEvent(int cityID, int numberOfActionsSpent): base(Game.theGame.CurrentPlayer)
    {
        this.oldCityID = _player.GetCurrentCity();
        this.newCityID = cityID;
        this.numberOfActionsSpent = numberOfActionsSpent;
    }

    public override void Do(Timeline timeline)
    {
        _player.UpdateCurrentCity(newCityID);
        Game.theGame.CurrentPlayer.ActionsRemaining -= numberOfActionsSpent;
       if(Game.theGame.CurrentPlayer.ActionsRemaining == 0)
        {
            Timeline.theTimeline.addEvent(new PEndTurn());
        }
    }

    public override float Act(bool qUndo = false)
    {
        Game.theGame.Cities[newCityID].draw();
        Game.theGame.Cities[oldCityID].draw();
        _playerGui.ClearSelectedAction();
        _playerGui.draw();
        return 0;
    }
}