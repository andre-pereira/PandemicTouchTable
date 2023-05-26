

using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Timeline.TimelineAsset;
using Unity.VisualScripting;

public class PFlyToCity : PlayerEvent
{
    int flyFrom;
    private Vector3 originalCardPosition;
    private Quaternion originalCardRotation;
    int flyTo;
    const float ANIMATIONDURATION = 1f;

    public PFlyToCity(int flyTo) : base(Game.theGame.CurrentPlayer)
    {
        this.flyTo = flyTo;
        flyFrom = _player.GetCurrentCity();
        originalCardPosition = _playerGui.getCardInHand(flyTo).transform.position;
        originalCardRotation = _playerGui.getCardInHand(flyTo).transform.rotation;
    }

    public override void Do(Timeline timeline)
    {
        _player.CardsInHand.Remove(flyTo);
        _player.UpdateCurrentCity(flyTo);
        game.PlayerCardsDiscard.Add(flyTo);
        game.CurrentPlayer.ActionsRemaining -= 1;
        if (game.CurrentPlayer.ActionsRemaining == 0)
        {
            Timeline.theTimeline.addEvent(new PEndTurn());
        }
    }

    public override float Act(bool qUndo = false)
    {
        _playerGui.draw();
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        GameObject cardToAddObject = _playerGui.AddPlayerCardToTransform(flyTo, gui.PlayerDeckDiscard.transform, false);
        cardToAddObject.transform.position = originalCardPosition;
        cardToAddObject.transform.rotation = originalCardRotation;
        sequence.Append(cardToAddObject.transform.DOMove(gui.PlayerDeckDiscard.transform.position, ANIMATIONDURATION));
        sequence.Join(cardToAddObject.transform.DORotate(gui.PlayerDeckDiscard.transform.eulerAngles, ANIMATIONDURATION));
        sequence.AppendCallback(() =>
        {
            gui.drawBoard();
        });
        
        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(cardToAddObject.transform.DOShakeRotation(durationMove / 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        //sequence.Append(cardToAddObject.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), durationMove)).
        //    Join(cardToAddObject.transform.DOMove(new Vector3(0, 0, 0), durationMove));
        //sequence.AppendInterval(durationMove);
        //sequence.Append(cardToAddObject.transform.DOScale(new Vector3(1f, 1f, 1f), durationMove)).
        //    Join(cardToAddObject.transform.DOMove(gui.InfectionDiscard.transform.position, durationMove));

        City currentCity = Game.theGame.Cities[flyFrom];
        City cityToMoveTo = Game.theGame.Cities[flyTo];
        currentCity.removePawn(_player);
        currentCity.draw();
        GameObject movingPawn = Object.Instantiate(gui.PawnPrefab, currentCity.transform.position, currentCity.transform.rotation, gui.AnimationCanvas.transform);
        movingPawn.GetComponent<Image>().color = _playerGui.roleCard.RoleCardData.roleColor;
        movingPawn.GetComponent<Outline>().enabled = true;
        sequence.Append(movingPawn.transform.DOMove(cityToMoveTo.transform.position, ANIMATIONDURATION).OnComplete(() =>
        {
            cityToMoveTo.draw();
            Object.Destroy(movingPawn);
        }));
        return sequence.Duration();
    }
}