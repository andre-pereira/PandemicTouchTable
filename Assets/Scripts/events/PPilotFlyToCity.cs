internal class PPilotFlyToCity : PlayerEvent
{
    private int pilotCitySelected;
    private int initialOtherPlayerCity = -1;
    private int initialPlayerCity;
    private Player otherPlayer;

    public PPilotFlyToCity(int pilotCitySelected, Player otherPlayer) : base(Game.theGame.CurrentPlayer)
    {
        this.pilotCitySelected = pilotCitySelected;
        this.otherPlayer = otherPlayer;
    }

    public override void Do(Timeline timeline)
    {
        initialPlayerCity = _player.GetCurrentCity();
        _player.UpdateCurrentCity(pilotCitySelected, true);
        if (otherPlayer != null)
        {
            initialOtherPlayerCity = otherPlayer.GetCurrentCity();
            otherPlayer.UpdateCurrentCity(pilotCitySelected, true);
        }
        _player.DecreaseActionsRemaining(1);
    }

    public override float Act(bool qUndo = false)
    {
        if (initialOtherPlayerCity != -1 && otherPlayer != null)
            Game.theGame.Cities[initialOtherPlayerCity].draw();
        Game.theGame.Cities[pilotCitySelected].draw();
        Game.theGame.Cities[initialPlayerCity].draw();
        _playerGui.draw();
        return 0;
    }

    public override string GetLogInfo()
    {
        return $@" ""pilotCitySelected"" : ""{pilotCitySelected}"",
                    ""otherPlayer"" : ""{otherPlayer.Role}"",
                ";
    }
}
