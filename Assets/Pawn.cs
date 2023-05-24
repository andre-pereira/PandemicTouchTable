using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pawn : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Canvas citiesCanvas;
    private Canvas canvas;
    private Vector2 offset;
    private RectTransform rectTransform;
    private GraphicRaycaster graphicRaycaster;
    public bool canMove = false;
    
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
            Debug.Log("Ended in city: " + endedInCity.name);
        }
    }
}
