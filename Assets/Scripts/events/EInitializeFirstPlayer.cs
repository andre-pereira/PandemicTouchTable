using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static GameGUI;
using static Game;

internal class EInitializeFirstPlayer : EngineEvent
{

    public EInitializeFirstPlayer()
    {
        QUndoable = true;

    }

    public override void Do(Timeline timeline)
    {

        Player startPlayer = PlayerList.Players.GetRandom();
        int highestPopulation = 0;
        foreach (Player player in PlayerList.Players)
        {
            foreach (int card in player.CityCardsInHand)
            {
                int population = int.Parse(theGame.Cities[card].city.population.Replace(".", ""));
                if (population > highestPopulation)
                {
                    highestPopulation = population;
                    startPlayer = player;
                }
            }
        }

        PlayerList.setOrderToClockwiseWithStartAt(startPlayer);
        theGame.CurrentPlayer = startPlayer;
        startPlayer.ResetTurn();
    }

    public override float Act(bool qUndo = false)
    {
        gui.drawCurrentPlayerArea();
        return 0f;
    }
}