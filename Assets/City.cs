using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public CityCard city;

    private Player.Roles?[] availablePawnSlotsInCity = new Player.Roles?[4];

    // Start is called before the first frame update
    void Start()
    {
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
        Vector3 toReturn = new Vector3(0, 0, 0);
        for (int i = 0; i < availablePawnSlotsInCity.Length; i++)
        {
            if (availablePawnSlotsInCity[i] == role)
            {
                
                toReturn.x = gameObject.transform.position.x + GameGUI.theGameGUI.PawnPositionInCityOffset[i].x;
                //Debug.Log("x... city:" + this.name+ " Role:" + role + " City position:"+ gameObject.transform.localPosition.x +  " PawnPositionInCityOffset: " + GameGUI.theGameGUI.PawnPositionInCityOffset[i].x);

                toReturn.y = gameObject.transform.position.y + GameGUI.theGameGUI.PawnPositionInCityOffset[i].y;
                //Debug.Log("y... city:" + this.name + " Role:" + role + " City position:" + gameObject.transform.position.y + " PawnPositionInCityOffset: " + GameGUI.theGameGUI.PawnPositionInCityOffset[i].y);
            }
        }
        return toReturn;
    }
}
