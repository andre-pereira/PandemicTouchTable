using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public static readonly float[][] offsetCubes = new float[][]
    {
        new float[] { 0.28f, -0.48f },
        new float[] { 0.48f, -0.48f },
        new float[] { 0.08f, -0.38f },
        new float[] { 0.58f, -0.38f }
    };

    public static readonly float[][] offsetPawns = new float[][]
{
        new float[] { -0.1f, 0.55f },
        new float[] { 0.1f, 0.55f },
        new float[] { 0.3f, 0.45f },
        new float[] { -0.3f, 0.45f }
};

    private Game game;
    private GameGUI gui;

    public CityCard city;

    public int numberOfInfectionCubes = 0;
    public GameObject CubesGameObject;
    public GameObject PawnsGameObject;

    public List<Player> PlayersInCity = new List<Player>();
    public Pawn[] PawnsInCity;

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;


    void Start()
    {
        game = Game.theGame;
        gui = GameGUI.gui;
        PawnsInCity = new Pawn[4];
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void addPawn(Player player)
    {
        PlayersInCity.Add(player);
    }

    public void removePawn(Player player)
    {
        PlayersInCity.Remove(player);
    }

    public void draw()
    {
        Pawn[] Pawns = new Pawn[4];
        Vector3 worldPoint = new Vector3(0, 0, 0);
        CubesGameObject.DestroyChildrenImmediate();
        PawnsGameObject.DestroyChildrenImmediate();

        if (numberOfInfectionCubes > 0)
        {
            for (int i = 0; i < numberOfInfectionCubes; i++)
            {
                GameObject cube = Instantiate(gui.cubePrefab, CubesGameObject.transform);
                cube.transform.Translate(offsetCubes[i][0], offsetCubes[i][1],0);
                cube.GetComponent<Cube>().virusInfo = city.virusInfo;
            }
        }

        if (PlayersInCity.Count > 0)
        {
            for (int i = 0; i < PlayersInCity.Count; i++)
            {
                int playerPosition = PlayersInCity[i].Position;
                GameObject pawn = Instantiate(gui.PawnPrefab, PawnsGameObject.transform.position , PawnsGameObject.transform.rotation, PawnsGameObject.transform);
                pawn.transform.Translate(offsetPawns[i][0], offsetPawns[i][1], 0);
                pawn.GetComponent<Image>().color = gui.roleCards[(int)PlayersInCity[i].Role].roleColor;
                PawnsInCity[playerPosition] = pawn.GetComponent<Pawn>();
            }
        }
        //GameObject currentPawn = gui.Pawns[(int)PlayerRole];
        //currentPawn.SetActive(true);
        //Image currentPawnImage = currentPawn.GetComponent<Image>();
        //City initialCity = game.Cities[game.InitialCityID];
        //Vector3 pawnPosition = initialCity.getPawnPosition(PlayerRole);
    }
}
