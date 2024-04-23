internal class PMobilizeEvent : PlayerEvent
{
    private int newCityID;
    private int oldCityID;

    public PMobilizeEvent(Player playerModel, int cityID): base(playerModel)
    {
        this.oldCityID = _player.GetCurrentCity();
        this.newCityID = cityID;
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



}