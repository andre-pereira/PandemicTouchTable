using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public CityCard city;

    private Player.Roles?[] availablePawnSlotsInCity = new Player.Roles?[4];

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;


    // Start is called before the first frame update
    void Start()
    {
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

}
