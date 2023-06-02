using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ENUMS;
using static GameGUI;
using static Game;

internal class EDrawInfectionCard : EngineEvent
{
    private const float ANIMATIONDURATION = 0.5f;
    private const float scaleToCenterScale = 3f;

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
            numberOfCityToInfect = theGame.InfectionCards.Pop();
        else
        {
            numberOfCityToInfect = theGame.InfectionCards[0];
            theGame.InfectionCards.Remove(numberOfCityToInfect);
        }

        theGame.InfectionCardsDiscard.Add(numberOfCityToInfect);
        if (theGame.InfectionCards.Count == 0) 
        {
            theGame.InfectionCards = theGame.InfectionCardsDiscard;
            theGame.InfectionCardsDiscard = new List<int>();
            theGame.InfectionCards.Shuffle();
        }
        cityToInfect = theGame.Cities[numberOfCityToInfect];

        if(checkIfNoMoreCubesExist(cityToInfect))
        {
            Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
            return;
        }

        bool outbreak = cityToInfect.incrementNumberOfCubes(cityToInfect.city.virusInfo.virusName, numberOfCubes);

        if(outbreak)
        { 
            if(theGame.OutbreakTracker.Contains(cityToInfect.city.cityID) == false)
                Timeline.theTimeline.addEvent(new EOutbreak(cityToInfect));
            else theGame.actionCompleted = true;
        }
        else theGame.actionCompleted = true;
    }


    private bool checkIfNoMoreCubesExist(City cityToInfect)
    {
        bool gameOver = false;
        switch (cityToInfect.city.virusInfo.virusName)
        {
            case VirusName.Red:
                theGame.RedCubesOnBoard-= numberOfCubes;
                if (theGame.RedCubesOnBoard < 0)
                {
                    Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
                    gameOver = true;
                }
                break;
            case VirusName.Yellow:
                theGame.YellowCubesOnBoard -= numberOfCubes;
                if (theGame.YellowCubesOnBoard < 0)
                {
                    Timeline.theTimeline.addEvent(new EGameOver(GameOverReasons.NoMoreCubesOfAColor));
                    gameOver = true;
                }
                break;
            case VirusName.Blue:
                theGame.BlueCubesOnBoard -= numberOfCubes;
                if (theGame.BlueCubesOnBoard < 0)
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
        sequence.Append(cardToAddObject.transform.DOShakeRotation(ANIMATIONDURATION / 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), ANIMATIONDURATION)).
            Join(cardToAddObject.transform.DOMove(new Vector3(0, 0, 0), ANIMATIONDURATION));
        sequence.AppendInterval(ANIMATIONDURATION);
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(1f, 1f, 1f), ANIMATIONDURATION)).
            Join(cardToAddObject.transform.DOMove(gui.InfectionDiscard.transform.position, ANIMATIONDURATION));

        sequence.Append(cardToAddObject.transform.DOShakeRotation(ANIMATIONDURATION * 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        List<GameObject> cubes = new List<GameObject>();
        for (int i = 0; i < numberOfCubes; i++)
        {
            GameObject cubeToDuplicate = gui.GetCubeToDuplicate(cityToInfect.GetComponent<City>().city.virusInfo, i);
            cubes.Add(Object.Instantiate(cubeToDuplicate, gui.AnimationCanvas.transform));
            cubes[i].transform.position = cubeToDuplicate.transform.position;
            cubes[i].transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            cubes[i].SetActive(true);
            Vector3 positionToMove = new Vector3(cityToInfect.CubesGameObject.transform.position.x, cityToInfect.CubesGameObject.transform.position.y, 0);
            sequence.Join(cubes[i].transform.DOMove(positionToMove, ANIMATIONDURATION * 2));
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
            gui.drawBoard();
            cityToInfect.draw();
        });

        return sequence.Duration();
    }

}


