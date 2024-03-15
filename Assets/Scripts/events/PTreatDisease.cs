using UnityEngine;
using static ENUMS;

public class PTreatDisease : PlayerEvent
{
    private City city;
    private VirusName virusName;
    private bool defaultClick = true;

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
        VirusName? virus = city.firstVirusFoundInCity();

        if (defaultClick)
            virus = virusName;
        
        if (virus != null)
        {
            if ((game.RedCure && virus == VirusName.Red)
                || (game.BlueCure && virus == VirusName.Blue) 
                || (game.YellowCure && virus == VirusName.Yellow))
            {
                city.resetCubesOfColor((VirusName)virus);
                Debug.Log("A cure has been found!");
            }
            city.incrementNumberOfCubes((VirusName)virus, -1);
            game.incrementNumberOfCubesOnBoard((VirusName)virus, 1);
        }

        _player.DecreaseActionsRemaining(1);
    }

    public override float Act(bool qUndo = false)
    {
        city.draw();
        _playerGui.draw();
        gameGUI.drawBoard();
        return 0;
    }
}