using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static Game;
using static GameGUI;

internal class EOutbreak : EngineEvent
{
    private City originOfOutbreak;
    private float ANIMATIONDURATION = 2f / gui.AnimationTimingMultiplier;

    public EOutbreak(City origin)
    {
        this.originOfOutbreak = origin;
    }

    public override void Do(Timeline timeline)
    {
        bool recurrentOutbreak = false;
        if(theGame.CurrentGameState != GameState.OUTBREAK)
            theGame.setCurrentGameState(GameState.OUTBREAK);
        theGame.OutbreakTracker.Add(originOfOutbreak.city.cityID);
        foreach (int neighbor in originOfOutbreak.city.neighbors)
        {
            City neighborCity = gui.Cities[neighbor].GetComponent<City>();
            if (neighborCity.incrementNumberOfCubes(originOfOutbreak.city.virusInfo.virusName, 1))
            {
                if (theGame.OutbreakTracker.Contains(neighborCity.city.cityID) == false)
                {
                    Timeline.theTimeline.addEvent(new EOutbreak(neighborCity));
                    recurrentOutbreak = true;
                }
            }
        }
        if(!recurrentOutbreak)
            theGame.actionCompleted = true;
    }

    public override float Act(bool qUndo = false)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(originOfOutbreak.transform.DOShakeRotation(ANIMATIONDURATION, new Vector3(0f, 0f, 3f), 10, 90, false));
        List<GameObject> listCubes = new List<GameObject>();
        foreach (int neighbor in originOfOutbreak.city.neighbors)
        {
            GameObject cube = Object.Instantiate(gui.cubePrefab, originOfOutbreak.transform.position, originOfOutbreak.transform.rotation, gui.AnimationCanvas.transform);
            cube.GetComponent<Cube>().virusInfo = originOfOutbreak.city.virusInfo;
            listCubes.Add(cube);
            sequence.Join(cube.transform.DOMove(gui.Cities[neighbor].transform.position, ANIMATIONDURATION));
        }
        sequence.AppendCallback(() =>
        {
            foreach (GameObject cube in listCubes)
            {
                Object.Destroy(cube);
            }
            foreach (int neighbor in originOfOutbreak.city.neighbors)
            {
                City neighborCity = gui.Cities[neighbor].GetComponent<City>();
                neighborCity.draw();
            }
        });
        return sequence.Duration();
    }
}