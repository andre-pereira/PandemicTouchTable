using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pawn : MonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler
{
    public Canvas citiesCanvas;
    private Canvas canvas;
    private Vector2 offset;
    private RectTransform rectTransform;
    private GraphicRaycaster graphicRaycaster;
    public bool canMove = false;
    private Vector2 initialPosition;
    
    private City endedInCity = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (canMove)
        {
            Vector2 pointerPosition = eventData.position;
            Vector2 localPointerPosition;
            endedInCity = null;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, pointerPosition, canvas.worldCamera, out localPointerPosition))
            {
                rectTransform.localPosition = localPointerPosition - offset;
            }


            GraphicRaycaster raycaster = citiesCanvas.GetComponent<GraphicRaycaster>();

            List<RaycastResult> results = new List<RaycastResult>();

            raycaster.Raycast(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "Image")
                {
                    this.gameObject.transform.position = result.gameObject.transform.position;
                    endedInCity = result.gameObject.GetComponentInParent<City>();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(endedInCity != null)
        {
            int numberOfActionsLeft = Game.theGame.CurrentPlayer.ActionsRemaining;
            List<int> citiesToVisit = new List<int>();
            List<int> citiesVisited = new List<int>();
            bool foundConnection = false;

            rectTransform.localPosition = initialPosition;
            List<int> newCitiesToVisit = new List<int>();

            foreach (int city in endedInCity.city.neighbors)
            {
                newCitiesToVisit.Add(city);
            }

            for (int i = 0; i < Game.theGame.CurrentPlayer.ActionsRemaining; i++)
            {
                citiesToVisit = new List<int>(newCitiesToVisit);
                newCitiesToVisit.Clear();
                foreach (int city in citiesToVisit)
                {
                    numberOfActionsLeft--;
                    if(citiesVisited.Contains(city))
                    {
                        continue;
                    }
                    else
                    {
                        if (Game.theGame.CurrentPlayer.CurrentCity == city)
                        {
                            foundConnection = true;
                            break;
                        }
                        else
                        {
                            foreach (int neighbor in Game.theGame.Cities[city].city.neighbors)
                            {
                                newCitiesToVisit.Add(neighbor);
                            }
                            citiesVisited.Add(city);
                        }
                    }
                }
                if(foundConnection)
                {
                    break;
                }
            }

            if(foundConnection && numberOfActionsLeft >= 0)
            {
                Game.theGame.CurrentPlayer.CurrentCity = endedInCity.city.cityID;
                endedInCity.draw();
                Game.theGame.CurrentPlayer.ActionsRemaining = numberOfActionsLeft;
                Debug.Log("Ended in city: " + endedInCity.name);
                
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out initialPosition);
    }
}
