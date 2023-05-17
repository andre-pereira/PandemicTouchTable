using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class EInitializeFirstPlayer : EngineEvent
{
    Game game = Game.theGame;
    GameGUI gui = GameGUI.theGameGUI;

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
            foreach(int card in player.CardsInHand)
            {
                if (card < 24)
                {
                    int population = int.Parse(gui.Cities[card].GetComponent<City>().city.population.Replace(".",""));
                    if (population > highestPopulation)
                    {
                        highestPopulation = population;
                        startPlayer = player;
                    }
                }
            }
        }

        PlayerList.setOrderToClockwiseWithStartAt(startPlayer);
        game.CurrentPlayer = startPlayer;
    }
}