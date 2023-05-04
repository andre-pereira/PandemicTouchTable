using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PTakeCard : PlayerEvent {
  public PTakeCard() : base(Game.theGame.CurrentPlayer) { }

  private int _numChips = 0;
  public override void Do(Timeline timeline)
  {
    _numChips = Game.theGame.NumCenterChips;
    _player.NumChips += Game.theGame.NumCenterChips;
    Game.theGame.NumCenterChips = 0;
    _player.TakeGift(Game.theGame.GiftDeck.Pop(0));

    if (Game.theGame.GiftDeck.Count == 0)
      timeline.addEvent(new EEndGame());
  }
  public override float Act(bool qUndo = false)
  {
    // Center card
    GameObject movingCard = GameGUI.cloneOnCanvas(GameGUI.theGameGUI.OfferedGift.gameObject);
    movingCard.transform.DOMove(_gui.CardContainer.transform.position, 1f).
      OnComplete(() =>
      {
        GameObject.Destroy(movingCard);
        _gui.draw();
      });
    movingCard.transform.DORotate(_gui.CardContainer.transform.rotation.eulerAngles, 1f);

    // Center chips
    if ( _numChips > 0 )
    {
      GameObject movingChip = GameGUI.cloneOnCanvas(GameGUI.theGameGUI.OfferedChips);
      movingChip.transform.DOMove(_gui.Chips.transform.position, 1f).
        OnComplete(() => GameObject.Destroy(movingChip));
      AudioPlayer.RepeatClip(AudioPlayer.AudioClipEnum.CHIP, _numChips);
    }

    GameGUI.theGameGUI.drawCenter();

    // New center card
    AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CARD_FLIP, 1f);
    GameGUI.theGameGUI.OfferedGift.transform.DOMove(GameGUI.theGameGUI.DeckSizeText.transform.position, 1f).
      From().
      SetDelay(0.5f);

    return 0;
  }
}
