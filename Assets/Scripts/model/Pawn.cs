using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameGUI;
using static Game;
using System;

public class Pawn : MonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Canvas citiesCanvas;
    private Canvas canvas;
    private Vector2 offset;
    private RectTransform rectTransform;
    public bool CanMove = false;
    public bool IsInterfaceElement = false;
    private Vector2 initialPosition;
    private int initialCityID;
    
    private City endedInCity = null;
    public Player.Roles PawnRole;

    public Player PlayerModel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        citiesCanvas = GameGUI.gui.CityCanvas.GetComponent<Canvas>();
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (CanMove)
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
        PlayerGUI pGUI = currentPlayerPad();
        if (endedInCity != null && endedInCity.city.cityID != theGame.CurrentPlayer.GetCurrentCity())
        {
            if (pGUI.ActionSelected == ActionTypes.Charter)
            {
                Timeline.theTimeline.addEvent(new PCharterEvent(endedInCity));
                Destroy(gameObject);
                return;
            }
            else
            {
                int distance = theGame.DistanceFromCity(this.PlayerModel.GetCurrentCity(), endedInCity.city.cityID);
                if (pGUI.PInEventCard == EventState.CALLTOMOBILIZE)
                {
                    if(distance > 0 && distance <= 2)
                    {
                        Timeline.theTimeline.addEvent(new PMobilizeEvent(this.PlayerModel, endedInCity.city.cityID));
                        Destroy(this.gameObject);
                    }
                    else rectTransform.localPosition = initialPosition;
                }
                else if (pGUI.ActionSelected == ActionTypes.Move)
                {
                    if (distance > 0 && distance <= pGUI.PlayerModel.ActionsRemaining)
                    {
                        Timeline.theTimeline.addEvent(new PMoveEvent(endedInCity.city.cityID, distance));
                        Destroy(this.gameObject);
                    }
                    else rectTransform.localPosition = initialPosition;
                }
            }
        }
        else rectTransform.localPosition = initialPosition;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = rectTransform.localPosition;
        initialCityID = theGame.CurrentPlayer.GetCurrentCity();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsInterfaceElement)
            theGame.CurrentPlayer.playerGui.PawnClicked(this);
    }

    internal void SetRoleAndPlayer(Player player)
    {
        PawnRole = player.Role;
        PlayerModel = player;
        GetComponent<Image>().color = gui.roleCards[(int)PawnRole].roleColor;
    }
}
