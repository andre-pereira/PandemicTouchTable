using static Game;

internal class PForecast : PlayerEvent
{
    public PForecast(Player playerModel): base(playerModel){}

    public override float Act(bool qUndo = false)
    {
        _playerGui.ClearSelectedAction();
        _playerGui.draw();
        gameGUI.draw();
        return 0;
    }

    public override void Do(Timeline timeline)
    {
        _playerGui.ForeCastEventCardsIDs.Reverse();
        foreach (var item in _playerGui.ForeCastEventCardsIDs)
        {
            theGame.InfectionCards.Add(item);
        }

        _playerGui.ForeCastEventCards[0].transform.parent.gameObject.SetActive(false);
        _playerGui.ForeCastEventCardsIDs.Clear();
        _playerGui.ForeCastEventCardSelected = -1;
        _playerGui.ChangeToInEvent(EventState.NOTINEVENT, false);
    }
}