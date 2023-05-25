using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pawn : MonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler
{
    private Canvas citiesCanvas;
    private Canvas canvas;
    private Vector2 offset;
    private RectTransform rectTransform;
    private GraphicRaycaster graphicRaycaster;
    public bool canMove = false;
    private Vector2 initialPosition;
    private int initialCityID;
    
    private City endedInCity = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        citiesCanvas = GameGUI.gui.CityCanvas.GetComponent<Canvas>();
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
        if (endedInCity != null)
        {
            if (endedInCity.city.cityID == Game.theGame.CurrentPlayer.GetCurrentCity())
                return;

            int numberOfActionsSpent = 0;
            HashSet<int> citiesToVisit = new HashSet<int>();
            HashSet<int> citiesVisited = new HashSet<int>();
            bool foundConnection = false;

            HashSet<int> newCitiesToVisit = new HashSet<int>();

            newCitiesToVisit.UnionWith(endedInCity.city.neighbors);

            for (int i = 0; i < Game.theGame.CurrentPlayer.ActionsRemaining; i++)
            {
                numberOfActionsSpent++;
                citiesToVisit = new HashSet<int>(newCitiesToVisit);
                newCitiesToVisit.RemoveWhere(citiesVisited.Contains);
                citiesVisited.UnionWith(citiesToVisit);

                if (citiesVisited.Contains(Game.theGame.CurrentPlayer.GetCurrentCity()))
                {
                    foundConnection = true;
                    break;
                }

                foreach (int city in citiesToVisit)
                {
                    foreach (int neighbor in Game.theGame.Cities[city].city.neighbors)
                    {
                        newCitiesToVisit.Add(neighbor);
                    }
                }
            }

            if (foundConnection)
            {
                Timeline.theTimeline.addEvent(new PMoveEvent(endedInCity.city.cityID, numberOfActionsSpent));
                Destroy(this.gameObject);
            }
            else rectTransform.localPosition = initialPosition;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out initialPosition);
        initialCityID = Game.theGame.CurrentPlayer.GetCurrentCity();
    }
}
