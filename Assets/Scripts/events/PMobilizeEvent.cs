internal class PMobilizeEvent : PlayerEvent
{
    private int newCityID;
    private int oldCityID;

    private Player _targetPlayer;

    public PMobilizeEvent(Player playerModel, int cityID): base(playerModel)
    {
        _targetPlayer = playerModel;
        oldCityID = _player.GetCurrentCity();
        newCityID = cityID;
    }

    public override float Act(bool qUndo = false)
    {
        Game.theGame.Cities[newCityID].draw();
        Game.theGame.Cities[oldCityID].draw();
        _playerGui.draw();
        return 0;
    }

    public override void Do(Timeline timeline)
    {
        _player.UpdateCurrentCity(newCityID, true);
        _playerGui.eventExecuted = true;
        _player.playerGui.ChangeToInEvent(Game.EventState.NOTINEVENT, true);
    }
    
    public override string GetLogInfo()
    {
        return $@" ""player"" : {_targetPlayer.Role}
                    ""newCity"" : {newCityID},
                    ""oldCity"" : {_targetPlayer.GetCurrentCity()},
                ";
    }



}