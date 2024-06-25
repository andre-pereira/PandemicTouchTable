using static ENUMS;

internal class PContainSpecialistRemoveWhenEntering : PlayerEvent
{
    private City city;

    private int redCount, yellowCount, blueCount;

    private VirusName cubeRemoved;
    
    float ANIMATIONDURATION = 1f / GameGUI.gui.AnimationTimingMultiplier;

    public PContainSpecialistRemoveWhenEntering(City city, int redCount, int yellowCount, int blueCount) : base(Game.theGame.CurrentPlayer)
    {
        this.city = city;
        this.redCount = redCount;
        this.yellowCount = yellowCount;
        this.blueCount = blueCount;
    }

    public override void Do(Timeline timeline)
    {
        
        if (redCount >= 2)
        {
            city.incrementNumberOfCubes(VirusName.Red, -1);
            game.incrementNumberOfCubesOnBoard(VirusName.Red, 1);
            cubeRemoved = VirusName.Red;
        }

        if (yellowCount >= 2)
        {
            city.incrementNumberOfCubes(VirusName.Yellow, -1);
            game.incrementNumberOfCubesOnBoard(VirusName.Yellow, 1);
            cubeRemoved = VirusName.Yellow;
        }

        if (blueCount >= 2)
        {
            city.incrementNumberOfCubes(VirusName.Blue, -1);
            game.incrementNumberOfCubesOnBoard(VirusName.Blue, 1);
            cubeRemoved = VirusName.Blue;
        }
    }

    public override float Act(bool qUndo = false)
    {
        city.draw();
        _playerGui.draw();
        gameGUI.drawBoard();
        return 0f;
    }
    
    public override string GetLogInfo()
    {
        return $@" ""city"" : {city.city.cityID},
                     ""cubeRemoved"" : ""{cubeRemoved}""
                ";
    }
}