using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using System;

public class PlayerGUI : MonoBehaviour {

  Player _player = null;
  public Player Player
  {
    get { if (_player == null) _player = PlayerList.playerAtPosition(Position); return _player; }
    set { _player = value; }
  }

  public int Position;

  public GameObject PlayerDialogPrefab;

  [HideInInspector]
  public PlayerDialogGUI DialogGUI;

  public TMPro.TextMeshProUGUI TotalScore;
  public GameObject Chips;
  public GameObject CardPrefab;
  public GameObject SpacerPrefab;
  public Transform CardContainer;

  void OnEnable()
  {
  }
  public void init()
  {
    _player = null;
    string dialogsName = "playerDialogs_" + Position;
    Transform dialogCanvas = GameObject.FindGameObjectWithTag("dialogCanvas").transform;
    if (!dialogCanvas.Find(dialogsName))
    {
      GameObject dialogs = Instantiate(PlayerDialogPrefab);
      dialogs.name = dialogsName;
      dialogs.transform.SetParent(dialogCanvas, false);
      dialogs.transform.SetSiblingIndex(0);
      RectTransform newRect = dialogs.GetComponent<RectTransform>();
      RectTransform oldRect = transform.GetComponent<RectTransform>();
      newRect.anchorMax = oldRect.anchorMax;
      newRect.anchorMin = oldRect.anchorMin;
      newRect.offsetMax = oldRect.offsetMax;
      newRect.offsetMin = oldRect.offsetMin;
      newRect.pivot = oldRect.pivot;
      newRect.anchoredPosition = oldRect.anchoredPosition;
      newRect.rotation = oldRect.rotation;

      DialogGUI = dialogs.GetComponent<PlayerDialogGUI>();
      DialogGUI.ParentGUI = this;
    }
  }

  bool _isAnimating = false;
  List<int> _drawnCards = new List<int>();
  public void draw()
  {
    if (_isAnimating) return;
    if (Player == null) return;

    //GetComponent<Image>().color = Player.solidColor();

    foreach ( var text in Chips.GetComponentsInChildren<TMPro.TextMeshProUGUI>())
    {
    }


    //if ( Player.AcceptedGifts.Count != _drawnCards.Count)
    //{
    //  CardContainer.gameObject.DestroyChildrenImmediate();
    //  _drawnCards = new List<int>(Player.AcceptedGifts);
    //  int priorGift = -1;
    //  foreach (int gift in _drawnCards )
    //  {
    //    if ( priorGift != -1 && gift > priorGift+1 )
    //    {
    //      GameObject spacer = Instantiate(SpacerPrefab);
    //      spacer.transform.SetParent(CardContainer, false);
    //    }
    //    GameObject card = Instantiate(CardPrefab, CardContainer, false);
    //    card.GetComponent<CardGUI>().draw(gift, gift > priorGift + 1);

    //    priorGift = gift;
    //  }
    //}

    DialogGUI.draw();
  }
  public void drawLater(float time)
  {
    _isAnimating = true;
    this.ExecuteLater(time, doneAnimating);
  }
  void doneAnimating()
  {
    _isAnimating = false;
    draw();
  }
  public void OnHelpClick()
  {
    DialogGUI.showHelp();
  }
  public void OnPayChipClick()
  {
  }
}