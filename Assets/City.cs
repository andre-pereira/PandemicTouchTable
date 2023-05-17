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

    private Game game;
    private GameGUI gui;

    public CityCard city;

    public int numberOfInfectionCubes = 0;
    public GameObject cubesGameObject;

    private Player.Roles?[] availablePawnSlotsInCity = new Player.Roles?[4];

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;


    // Start is called before the first frame update
    void Start()
    {
        game = Game.theGame;
        gui = GameGUI.theGameGUI;
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addPawn(Player.Roles role)
    {
        for (int i = 0; i < availablePawnSlotsInCity.Length; i++)
        {
            if (availablePawnSlotsInCity[i] == null)
            {
                availablePawnSlotsInCity[i] = role;
                return;
            }
        }
    }

    public void removePawn(Player.Roles role)
    {
        for (int i = 0; i < availablePawnSlotsInCity.Length; i++)
        {
            if (availablePawnSlotsInCity[i] == role)
            {
                availablePawnSlotsInCity[i] = null;
                return;
            }
        }
    }

    public Vector3 getPawnPosition(Player.Roles role)
    {
        Vector3 worldPoint = new Vector3(0,0,0);

        for (int i = 0; i < availablePawnSlotsInCity.Length; i++)
        {
            if (availablePawnSlotsInCity[i] == role)
            {
                Vector2 offset = GameGUI.theGameGUI.PawnPositionInCityOffset[i];
                Vector3 newLocalPosition = rectTransform.localPosition + new Vector3(offset.x, offset.y, 0);
                worldPoint = rectTransform.parent.TransformPoint(newLocalPosition);
            }
        }
        return worldPoint;
    }

    internal void draw()
    {
        cubesGameObject.DestroyChildrenImmediate();
        if (numberOfInfectionCubes > 0)
        {
            for (int i = 0; i < numberOfInfectionCubes; i++)
            {
                GameObject cube = Instantiate(gui.cubePrefab, cubesGameObject.transform);
                cube.transform.Translate(offsetCubes[i][0], offsetCubes[i][1],0);
                cube.GetComponent<Cube>().virusInfo = city.virusInfo;
            }
        }
    }
}
