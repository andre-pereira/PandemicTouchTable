using static ENUMS;

public class PTreatDisease : PlayerEvent
{
    private City city;
    private VirusName virusName;
    private bool defaultClick = true;
    float ANIMATIONDURATION = 1f / GameGUI.gui.AnimationTimingMultiplier;

    public PTreatDisease(City city, VirusName virusName): base(Game.theGame.CurrentPlayer)
    {
        this.city = city;
        this.virusName = virusName;
    }

    public PTreatDisease(City city) : base(Game.theGame.CurrentPlayer)
    {
        this.city = city;
        defaultClick = false;
    }

    public override void Do(Timeline timeline)
    {
        if (defaultClick)
            city.incrementNumberOfCubes(virusName,-1);
        else
        {
            VirusName? virus = city.firstVirusFoundInCity();
            if (virus != null)
            {
                city.incrementNumberOfCubes((VirusName)virus, -1);
                game.incrementNumberOfCubesOnBoard((VirusName)virus, 1);
            }
        }    
        _player.ActionsRemaining -= 1;
        if (game.CurrentPlayer.ActionsRemaining == 0)
        {
            game.setCurrentGameState(Game.GameState.DRAW1STPLAYERCARD);
        }
    }

    public override float Act(bool qUndo = false)
    {
        city.draw();
        _playerGui.draw();
        gui.drawBoard();
        return 0;
    }
}