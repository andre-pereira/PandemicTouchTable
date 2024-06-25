using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

internal class PCharterEvent : PlayerEvent
{
    private City flyTo, flyFrom;
    private Vector3 originalCardPosition;
    private Quaternion originalCardRotation;
    float ANIMATIONDURATION = 1f / GameGUI.gui.AnimationTimingMultiplier;

    public PCharterEvent(City flyTo) : base(Game.theGame.CurrentPlayer)
    {
        this.flyTo = flyTo;
        flyFrom = game.Cities[_player.GetCurrentCity()];
        originalCardPosition = _playerGui.getCardInHand(flyFrom.city.cityID).transform.position;
        originalCardRotation = _playerGui.getCardInHand(flyFrom.city.cityID).transform.rotation;
    }

    public override void Do(Timeline timeline)
    {
        _player.RemoveCardInHand(flyFrom.city.cityID,true);
        _player.UpdateCurrentCity(flyTo.city.cityID,true);
        _player.DecreaseActionsRemaining(1);
        _playerGui.ActionSelected = ActionTypes.None;
    }

    public override float Act(bool qUndo = false)
    {
        _playerGui.ClearSelectedAction();
        _playerGui.draw();
        flyTo.draw();
        //gui.drawBoard();
        Sequence sequence = DOTween.Sequence();
        GameObject cardToAddObject = game.AddPlayerCardToTransform(flyFrom.city.cityID, gameGUI.PlayerDeckDiscard.transform, false, _playerGui);
        cardToAddObject.transform.position = originalCardPosition;
        cardToAddObject.transform.rotation = originalCardRotation;
        sequence.Append(cardToAddObject.transform.DOMove(gameGUI.PlayerDeckDiscard.transform.position, ANIMATIONDURATION));
        sequence.Join(cardToAddObject.transform.DORotate(gameGUI.PlayerDeckDiscard.transform.eulerAngles, ANIMATIONDURATION));
        sequence.AppendCallback(() =>
        {
            gameGUI.drawBoard();
            //_playerGui.draw();
        });

        return sequence.Duration();
    }

    public override string GetLogInfo()
    {
        return $@" ""flyFrom"" : {flyFrom.city.cityID},
                    ""flyTo"" : {flyTo.city.cityID}
                ";
    }
}