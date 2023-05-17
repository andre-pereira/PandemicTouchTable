using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class EAddEpidemicCards : EngineEvent
{
    private const float OffsetEpidemicCards = 2.5f;
    private const float DurationEpidemicMove = 1f;
    GameGUI gui = GameGUI.theGameGUI;
    Game game = Game.theGame;

    public EAddEpidemicCards()
    {
        QUndoable = true;
    }

    public override void Do(Timeline timeline)
    {
        game.PlayerCards = addEpidemicCards(game.PlayerCards);
    }

    public override float Act(bool qUndo = false)
    {
        float offset = 0;
        for (int i = 0; i < 3; i++)
        {
            GameObject epidemicCard = Object.Instantiate(gui.EpidemicCardPrefab, gui.AnimationCanvas.transform);
            epidemicCard.transform.rotation = gui.PlayerDeck.transform.rotation;
            epidemicCard.transform.Translate(new Vector3(offset - OffsetEpidemicCards, 0, 0));
            offset += OffsetEpidemicCards;
            epidemicCard.transform.DOMove(gui.PlayerDeck.transform.position, DurationEpidemicMove).OnComplete(() =>
            {
                gui.drawBoard();
                Object.Destroy(epidemicCard);
            });
        }

        return DurationEpidemicMove;
    }

    static List<int> addEpidemicCards(List<int> originalList)
    {
        int third = originalList.Count / 3;

        // Divide the list into three parts
        var part1 = originalList.Take(third).ToList();
        var part2 = originalList.Skip(third).Take(third).ToList();
        var part3 = originalList.Skip(third * 2).ToList();

        // Add the value to each part
        part1.Add(28);
        part2.Add(28);
        part3.Add(28);

        // Shuffle each part
        part1.Shuffle();
        part2.Shuffle();
        part3.Shuffle();

        // Join them back together
        var finalList = new List<int>();
        finalList.AddRange(part1);
        finalList.AddRange(part2);
        finalList.AddRange(part3);

        return finalList;
    }

}


