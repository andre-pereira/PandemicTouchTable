using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ENUMS;
using static UnityEngine.RuleTile.TilingRuleOutput;

internal class EDrawInfectionCard : EngineEvent
{
    private const float durationMove = 0.05f;
    private const float scaleToCenterScale = 3f;

    GameGUI gui = GameGUI.gui;
    Game game = Game.theGame;
    private int numberOfCubes;
    private int numberOfCityToInfect;
    private City cityToInfect;
    private bool fromTheTop;

    public EDrawInfectionCard(int numberOfCubes, bool fromTheTop)
    {
        this.fromTheTop = fromTheTop;
        QUndoable = true;
        this.numberOfCubes = numberOfCubes;
    }

    public override void Do(Timeline timeline)
    {
        if(fromTheTop)
            numberOfCityToInfect = game.InfectionCards.Pop();
        else
        {
            numberOfCityToInfect = game.InfectionCards[0];
            game.InfectionCards.Remove(numberOfCityToInfect);
        }

        game.InfectionCardsDiscard.Add(numberOfCityToInfect);
        if (game.InfectionCards.Count == 0) 
        {
            game.InfectionCards = game.InfectionCardsDiscard;
            game.InfectionCardsDiscard = new List<int>();
            game.InfectionCards.Shuffle();
        }
        cityToInfect = game.Cities[numberOfCityToInfect];

        if(checkIfNoMoreCubesExist(cityToInfect))
        {
            Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
            return;
        }

        cityToInfect.numberOfInfectionCubes += numberOfCubes;
        if(cityToInfect.numberOfInfectionCubes > 3)
        { 
            cityToInfect.numberOfInfectionCubes = 3;
            if(game.OutbreakTracker.Contains(cityToInfect.city.cityID) == false)
                Timeline.theTimeline.addEvent(new EOutbreak(cityToInfect));
            else game.actionCompleted = true;
        }
        else game.actionCompleted = true;
            
    }


    private bool checkIfNoMoreCubesExist(City cityToInfect)
    {
        bool gameOver = false;
        switch (cityToInfect.city.virusInfo.virusName)
        {
            case VirusName.Red:
                game.RedCubes--;
                if (game.RedCubes < 0)
                {
                    Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
                    gameOver = true;
                }
                break;
            case VirusName.Yellow:
                game.YellowCubes--;
                if (game.YellowCubes < 0)
                {
                    Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
                    gameOver = true;
                }
                break;
            case VirusName.Blue:
                game.BlueCubes--;
                if (game.BlueCubes < 0)
                {
                    Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
                    gameOver = true;
                }
                break;
            default:
                break;
        }
        return gameOver;
    }

    public override float Act(bool qUndo = false)
    {
        GameObject cardToAddObject = Object.Instantiate(gui.InfectionCardPrefab, gui.InfectionDeck.transform.position, gui.PlayerDeck.transform.rotation, gui.InfectionDiscard.transform);
        cardToAddObject.GetComponent<InfectionCardDisplay>().cityCardData = cityToInfect.city;
        //cardToAddObject.transform.position = gui.PlayerDeck.transform.position;
        //cardToAddObject.transform.rotation = gui.PlayerDeck.transform.rotation;

        gui.drawBoard();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(cardToAddObject.transform.DOShakeRotation(durationMove / 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), durationMove)).
            Join(cardToAddObject.transform.DOMove(new Vector3(0, 0, 0), durationMove));
        sequence.AppendInterval(durationMove);
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(1f, 1f, 1f), durationMove)).
            Join(cardToAddObject.transform.DOMove(gui.InfectionDiscard.transform.position, durationMove));

        sequence.Append(cardToAddObject.transform.DOShakeRotation(durationMove * 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        List<GameObject> cubes = new List<GameObject>();
        for (int i = 0; i < numberOfCubes; i++)
        {
            cubes.Add(gui.GetCubesList(cityToInfect.GetComponent<City>().city.virusInfo).Pop());
            Vector3 positionToMove = new Vector3(cityToInfect.CubesGameObject.transform.position.x, cityToInfect.CubesGameObject.transform.position.y, 0);
            sequence.Join(cubes[i].transform.DOMove(positionToMove, durationMove * 2));
            if (i == numberOfCubes - 1)
                sequence.AppendCallback(() =>
                {
                    foreach (GameObject cube in cubes)
                        Object.Destroy(cube);
                });
        }

        sequence.Play().OnComplete(() =>
        {
            //Object.Destroy(cardToAddObject);
            cityToInfect.draw();
        });

        return sequence.Duration();
    }

}


