using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ENUMS;
using static GameGUI;
using static Game;

public class City : MonoBehaviour
{
    public static readonly float[][] offsetCubesRed = new float[][]
    {
        new float[] { 0.61f, -0.42f },
        new float[] { 0.81f, -0.62f },
        new float[] { 0.61f, -0.82f }
    };

    public static readonly float[][] offsetCubesYellow = new float[][]
    {
        new float[] { 0.38f, -0.48f },
        new float[] { 0.30f, -0.68f },
        new float[] { 0.39f, -0.98f }
    };
    public static readonly float[][] offsetCubesBlue = new float[][]
    {
        new float[] { 0.15f, -0.42f },
        new float[] { 0.0f, -0.62f },
        new float[] { 0.15f, -0.82f }
    };

    public static readonly float[][] offsetPawns = new float[][]
    {
        new float[] { -0.1f, 0.55f },
        new float[] { 0.1f, 0.55f },
        new float[] { 0.3f, 0.45f },
        new float[] { -0.3f, 0.45f }
    };

    public CityCard city;

    private int numberOfInfectionCubesRed = 0;
    private int numberOfInfectionCubesYellow = 0;
    private int numberOfInfectionCubesBlue = 0;
    public GameObject CubesGameObject;
    public GameObject PawnsGameObject;

    public List<Player> PlayersInCity = new List<Player>();
    public Pawn[] PawnsInCity;

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;


    void Start()
    {
        PawnsInCity = new Pawn[4];
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public int getNumberOfCubes(VirusName virusName)
    {
        switch (virusName)
        {
            case VirusName.Red:
                return numberOfInfectionCubesRed;
            case VirusName.Yellow:
                return numberOfInfectionCubesYellow;
            case VirusName.Blue:
                return numberOfInfectionCubesBlue;
        }
        return 0;
    }

    public void resetCubesOfColor(VirusName virusName)
    {
        switch (virusName)
        {
            case VirusName.Red:
                resetRedCubes();
                break;
            case VirusName.Blue:
                resetBlueCubes();
                break;
            case VirusName.Yellow:
                resetYellowCubes();
                break;
        }
    }
    
    private void resetRedCubes()
    {
        numberOfInfectionCubesRed = 0;
    }

    private void resetYellowCubes()
    {
        numberOfInfectionCubesYellow = 0;
    }

    private void resetBlueCubes()
    {
        numberOfInfectionCubesBlue = 0;
    }

    public bool incrementNumberOfCubes(VirusName virusName, int increment)
    {
        switch (virusName)
        {
            case VirusName.Red:
                numberOfInfectionCubesRed += increment;
                if (numberOfInfectionCubesRed > 3)
                {
                    numberOfInfectionCubesRed = 3;
                    return true;
                }
                break;
            case VirusName.Yellow:
                numberOfInfectionCubesYellow += increment;
                if (numberOfInfectionCubesYellow > 3)
                {
                    numberOfInfectionCubesYellow = 3;
                    return true;
                }
                break;
            case VirusName.Blue:
                numberOfInfectionCubesBlue += increment;
                if (numberOfInfectionCubesBlue > 3)
                {
                    numberOfInfectionCubesBlue = 3;
                    return true;
                }
                break;
        }
        return false;
    }

    public void addPawn(Player player)
    {
        PlayersInCity.Add(player);
    }

    public void removePawn(Player player)
    {
        //remove a player and then sort the list so that null values are not first
        PlayersInCity.Remove(player);
    }

    public void draw()
    {
        Pawn[] Pawns = new Pawn[4];
        Vector3 worldPoint = new Vector3(0, 0, 0);
        CubesGameObject.DestroyChildrenImmediate();
        PawnsGameObject.DestroyChildrenImmediate();

        DrawCubes();

        if (PlayersInCity.Count > 0)
        {
            for (int i = 0; i < PlayersInCity.Count; i++)
            {
                int playerPosition = PlayersInCity[i].Position;
                GameObject pawn = Instantiate(gui.PawnPrefab, PawnsGameObject.transform.position, PawnsGameObject.transform.rotation, PawnsGameObject.transform);
                pawn.transform.Translate(offsetPawns[i][0], offsetPawns[i][1], 0);
                PawnsInCity[playerPosition] = pawn.GetComponent<Pawn>();
                PawnsInCity[playerPosition].SetRoleAndPlayer(PlayersInCity[i]);
            }
        }
        //GameObject currentPawn = gui.Pawns[(int)PlayerRole];
        //currentPawn.SetActive(true);
        //Image currentPawnImage = currentPawn.GetComponent<Image>();
        //City initialCity = game.Cities[game.InitialCityID];
        //Vector3 pawnPosition = initialCity.getPawnPosition(PlayerRole);
    }

    private void DrawCubes()
    {
        InstantiateCubes(numberOfInfectionCubesRed, offsetCubesRed, gui.VirusInfos[0]);
        InstantiateCubes(numberOfInfectionCubesYellow, offsetCubesYellow, gui.VirusInfos[1]);
        InstantiateCubes(numberOfInfectionCubesBlue, offsetCubesBlue, gui.VirusInfos[2]);
    }

    private void InstantiateCubes(int numberOfCubes, float[][] offsets, VirusInfo info)
    {
        for (int i = 0; i < numberOfCubes; i++)
        {
            GameObject cube = Instantiate(gui.cubePrefab, CubesGameObject.transform);
            cube.transform.Translate(offsets[i][0], offsets[i][1], 0);
            cube.GetComponent<Cube>().virusInfo = info;
            cube.GetComponentInChildren<Button>().onClick.AddListener(() => cubeClicked(info.virusName));
        }
    }

    private void cubeClicked(VirusName virusName)
    {
        theGame.CubeClicked(this, virusName);
    }

    public void Clicked()
    {
        GameGUI.currentPlayerPad().CityClicked(this);
    }

    public bool cubesInCity()
    {
        if (numberOfInfectionCubesRed > 0 || numberOfInfectionCubesYellow > 0 || numberOfInfectionCubesBlue > 0)
        {
            return true;
        }
        return false;
    }

    public VirusName? firstVirusFoundInCity()
    {
        if (numberOfInfectionCubesRed > 0)
            return VirusName.Red;
        if (numberOfInfectionCubesYellow > 0)
            return VirusName.Yellow;
        if (numberOfInfectionCubesBlue > 0)
            return VirusName.Blue;
        return null;
    }
}
