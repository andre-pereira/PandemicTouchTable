using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PPayChip : PlayerEvent
{
  public PPayChip() : base(Game.theGame.CurrentPlayer) { }

  public override void Do(Timeline timeline)
  {

    Game.theGame.CurrentPlayer = PlayerList.nextPlayer(_player);
  }

  public override float Act(bool qUndo = false)
  {
    AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CHIP);

    GameObject chipCopy = GameGUI.cloneOnCanvas(_gui.Chips);
    chipCopy.DestroyChildrenImmediate();
    chipCopy.transform.DOMove(GameGUI.theGameGUI.OfferedChips.transform.position, 1f).
      OnComplete(() =>
      {
        GameObject.Destroy(chipCopy);
        GameGUI.theGameGUI.drawCenter();
      });

    GameObject giftCopy = GameGUI.cloneOnCanvas(_gui.DialogGUI.GiftIcon);
    PlayerGUI nextPlayerGUI = GameGUI.currentPlayerPad();
    giftCopy.transform.DOMove(nextPlayerGUI.DialogGUI.GiftIcon.transform.position, 1f).
      OnComplete(() =>
      {
        GameObject.Destroy(giftCopy);
        nextPlayerGUI.draw();
      });
    giftCopy.transform.DORotate(nextPlayerGUI.transform.rotation.eulerAngles, 1f);

    _gui.draw();

    return 0;
  }
}
