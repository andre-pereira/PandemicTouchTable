internal class PMoveEvent : PlayerEvent
{
    private int newCityID;
    private int oldCityID;
    private int numberOfActionsSpent;
    float ANIMATIONDURATION = 1f / GameGUI.gui.AnimationTimingMultiplier;

    public PMoveEvent(int cityID, int numberOfActionsSpent): base(Game.theGame.CurrentPlayer)
    {
        this.oldCityID = _player.GetCurrentCity();
        this.newCityID = cityID;
        this.numberOfActionsSpent = numberOfActionsSpent;
    }

    public override void Do(Timeline timeline)
    {
        _player.UpdateCurrentCity(newCityID, true);
        _player.DecreaseActionsRemaining(numberOfActionsSpent);
    }

    public override float Act(bool qUndo = false)
    {
        Game.theGame.Cities[newCityID].draw();
        Game.theGame.Cities[oldCityID].draw();
        _playerGui.ClearSelectedAction();
        _playerGui.draw();
        return 0;
    }

    public override string GetLogInfo()
    {
        return $@" ""newCity"" : ""{newCityID}"",
                    ""oldCity"" : ""{oldCityID}"",
                    ""numberOfActionsSpent"" : ""{numberOfActionsSpent}""
                ";
    }
}