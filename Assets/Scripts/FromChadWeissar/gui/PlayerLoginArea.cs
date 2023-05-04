using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerLoginArea : MonoBehaviour {

  public MainMenu MainMenu;
  public GameObject ChosenColor;
  public TMPro.TextMeshProUGUI JoinText;
  public int Position;

  // Use this for initialization
  void Start() {
    reset();
  }

  public void reset()
  {
    _color = null;
    ChosenColor.SetActive(_color.HasValue);
    int i = 0;
    foreach (Image colorImg in transform.Find("colorButtons").GetComponentsInChildren<Image>())
    {
      colorImg.color = GameGUI.SolidColors[i++];
    }
  }
  public Player.PlayerColors? Color
  {
    get {return _color;}
    set
    {
      if (value != null)
      {
        Transform color = transform.Find("colorButtons").GetChild((int)value);
        ChosenColor.GetComponent<Image>().color = color.GetComponent<Image>().color;
      }
      _color = value;
    }
  }
  Player.PlayerColors? _color;
  public void OnColorClicked(int colorIndex)
  {
    AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);
    ChosenColor.SetActive(true);
    JoinText.text = "Tap to leave game";
    MainMenu.assignColorToPlayer(Position, (Player.PlayerColors)colorIndex);
  }
  public void OnJoinClicked()
  {
    AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);
    ChosenColor.SetActive(!ChosenColor.activeSelf);
    JoinText.text = (ChosenColor.activeSelf ? "Tap to leave game" : "Tap to join game");
    if (ChosenColor.activeSelf)
      MainMenu.assignFreeColorToPlayer(Position);
    else
      MainMenu.releaseColorFromPlayer(Position);
  }
  public bool isPlaying()
  {
    return ChosenColor.activeSelf;
  }
}
