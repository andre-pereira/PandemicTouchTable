using System.Collections.Generic;
using UnityEngine;
using static ENUMS;

internal class PContainSpecialistRemoveWhenEntering : PlayerEvent
{
    private City city;
    float ANIMATIONDURATION = 1f / GameGUI.gui.AnimationTimingMultiplier;

    public PContainSpecialistRemoveWhenEntering(int cityID) : base(Game.theGame.CurrentPlayer)
    {
        city = game.Cities[cityID].GetComponent<City>();
    }

    public override void Do(Timeline timeline)
    {

        int redCount = city.getNumberOfCubes(VirusName.Red);
        int yellowCount = city.getNumberOfCubes(VirusName.Yellow);
        int blueCount = city.getNumberOfCubes(VirusName.Blue);

        if (redCount >= 2)
        {
            city.incrementNumberOfCubes(VirusName.Red, -1);
            game.incrementNumberOfCubesOnBoard(VirusName.Red, 1);
        }

        if (yellowCount >= 2)
        {
            city.incrementNumberOfCubes(VirusName.Red, -1);
            game.incrementNumberOfCubesOnBoard(VirusName.Red, 1);
        }

        if (blueCount >= 2)
        {
            city.incrementNumberOfCubes(VirusName.Red, -1);
            game.incrementNumberOfCubesOnBoard(VirusName.Red, 1);
        }
    }

    public override float Act(bool qUndo = false)
    {
        city.draw();
        _playerGui.draw();
        gui.drawBoard();
        return 0f;
    }
}