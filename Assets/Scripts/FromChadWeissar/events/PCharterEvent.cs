using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

internal class PCharterEvent : PlayerEvent
{
    private City flyTo, flyFrom;
    private Vector3 originalCardPosition;
    private Quaternion originalCardRotation;
    const float ANIMATIONDURATION = 1f;

    public PCharterEvent(City flyTo) : base(Game.theGame.CurrentPlayer)
    {
        this.flyTo = flyTo;
        flyFrom = game.Cities[_player.GetCurrentCity()];
        originalCardPosition = _playerGui.getCardInHand(flyFrom.city.cityID).transform.position;
        originalCardRotation = _playerGui.getCardInHand(flyFrom.city.cityID).transform.rotation;
    }

    public override void Do(Timeline timeline)
    {
        _player.RemoveCityCardInHand(flyFrom.city.cityID);
        _player.UpdateCurrentCity(flyTo.city.cityID);
        game.PlayerCardsDiscard.Add(flyFrom.city.cityID);
        game.CurrentPlayer.ActionsRemaining -= 1;
        if (game.CurrentPlayer.ActionsRemaining == 0)
        {
            Timeline.theTimeline.addEvent(new PEndTurn());
        }
        _playerGui.ActionSelected = ActionTypes.None;
    }

    public override float Act(bool qUndo = false)
    {
        _playerGui.ClearSelectedAction();
        _playerGui.draw();
        //gui.drawBoard();
        Sequence sequence = DOTween.Sequence();
        GameObject cardToAddObject = _playerGui.AddPlayerCardToTransform(flyFrom.city.cityID, gui.PlayerDeckDiscard.transform, false);
        cardToAddObject.transform.position = originalCardPosition;
        cardToAddObject.transform.rotation = originalCardRotation;
        sequence.Append(cardToAddObject.transform.DOMove(gui.PlayerDeckDiscard.transform.position, ANIMATIONDURATION));
        sequence.Join(cardToAddObject.transform.DORotate(gui.PlayerDeckDiscard.transform.eulerAngles, ANIMATIONDURATION));
        sequence.AppendCallback(() =>
        {
            gui.drawBoard();
            flyTo.draw();
            //_playerGui.draw();
        });

        return sequence.Duration();
    }
}