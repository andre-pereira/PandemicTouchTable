using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ENUMS;

internal class EFlipCardAddCubes : EngineEvent
{
    private const float OffsetCubes = 2.5f;
    private const float DurationMove = 1f;
    GameGUI gui = GameGUI.theGameGUI;
    Game game = Game.theGame;
    private int numberOfCubes;
    private int numberOfCityToInfect;
    private City cityToInfect;

    public EFlipCardAddCubes(int numberOfCubes)
    {
        QUndoable = true;
        this.numberOfCubes = numberOfCubes;
    }

    public override void Do(Timeline timeline)
    {
        numberOfCityToInfect = game.InfectionCards.Pop();
        game.InfectionCardsDiscard.Add(numberOfCityToInfect);
        if (game.InfectionCards.Count == 0) 
        {
            game.InfectionCards = game.InfectionCardsDiscard;
            game.InfectionCardsDiscard = new List<int>();
            game.InfectionCards.Shuffle();
        }
        cityToInfect = gui.Cities[numberOfCityToInfect].GetComponent<City>();

        if(checkIfNoMoreCubesExist(cityToInfect))
        {
            Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
            return;
        }

        cityToInfect.numberOfInfectionCubes += numberOfCubes;
        if(cityToInfect.numberOfInfectionCubes > 4)
            cityToInfect.numberOfInfectionCubes = 4;

        if(cityToInfect.numberOfInfectionCubes == 4)
            Timeline.theTimeline.addEvent(new EOutbreak(cityToInfect));
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
        //GameObject cardToAddObject = playerGui.AddPlayerCardToTransform(cardToAdd, gui.AnimationCanvas.transform);
        //cardToAddObject.transform.position = gui.PlayerDeck.transform.position;
        //cardToAddObject.transform.rotation = gui.PlayerDeck.transform.rotation;

        //gui.drawBoard();
        Sequence sequence = DOTween.Sequence();
        //sequence.Append(cardToAddObject.transform.DOShakeRotation(scaleToCenterDuration / 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        //sequence.Append(cardToAddObject.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), scaleToCenterDuration)).
        //    Join(cardToAddObject.transform.DOMove(new Vector3(0, 0, 0), scaleToCenterDuration));
        //sequence.AppendInterval(scaleToCenterDuration);
        //sequence.Append(cardToAddObject.transform.DOScale(new Vector3(1f, 1f, 1f), scaleToCenterDuration)).
        //    Join(cardToAddObject.transform.DORotate(playerGui.transform.rotation.eulerAngles, scaleToCenterDuration)).
        //    Join(cardToAddObject.transform.DOMove(playerGui.PlayerCards.transform.position, scaleToCenterDuration)).
        //    OnComplete(() => {
        //        Object.Destroy(cardToAddObject);
        //        playerGui.draw();
        //    });
        //sequence.Play();


        //return sequence.Duration();
        cityToInfect.draw();
        return 0f;
    }

}


