using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PlayerDialogGUI : MonoBehaviour {

  PlayerGUI _parentGUI = null;
  public PlayerGUI ParentGUI { set {
      _parentGUI = value;
    } get { return _parentGUI; } }

  public Image Ribbon;
  public GameObject HelpDialog;
  public GameObject GiftIcon;

  void Awake()
  {
    Ribbon.gameObject.SetActive(false);
    HelpDialog.SetActive(false);
  }
  public void showHelp()
  {
    HelpDialog.SetActive(!HelpDialog.activeInHierarchy);
  }
  public void draw()
  {
    gameObject.SetActive(true);
    Ribbon.gameObject.SetActive(Game.theGame.CurrentGameState == Game.GameState.GAME_OVER);
    Ribbon.sprite = GameGUI.ribbonSprite(ParentGUI.Player.Place);

    GiftIcon.SetActive(Game.theGame.CurrentGameState == Game.GameState.PLAY &&
                       Game.theGame.CurrentPlayer == ParentGUI.Player);
  }

  public void OnAcceptGiftClick()
  {
    Timeline.theTimeline.addEvent(new PTakeCard());
  }
}
