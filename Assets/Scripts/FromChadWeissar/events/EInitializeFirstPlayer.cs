using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

internal class EInitializeFirstPlayer : EngineEvent
{
    Game game = Game.theGame;
    GameGUI gui = GameGUI.gui;

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
                    if(game.Cities[card].city.name == "Vienna")
                        Debug.Log("Vienna");
                    int population = int.Parse(game.Cities[card].city.population.Replace(".",""));
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
        startPlayer.ActionsRemaining = 4;
    }

    public override float Act(bool qUndo = false)
    {
        gui.drawCurrentPlayerArea();
        return 0f;
    }
}