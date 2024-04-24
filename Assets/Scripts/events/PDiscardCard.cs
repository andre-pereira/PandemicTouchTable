using DG.Tweening;
using System.Diagnostics;
using UnityEngine;

internal class PDiscardCard : PlayerEvent
{
    private float ANIMATIONDURATION = 1f / GameGUI.gui.AnimationTimingMultiplier;
    private int cardToDiscard;
    private PlayerGUI playerGui;
    private Vector3 objectToDiscardPosition;
    private Quaternion objectToDiscardRotation;

    public PDiscardCard(int cardID, PlayerGUI playerGui)
    {
        this.cardToDiscard = cardID;
        this.playerGui = playerGui;
    }

    public override void Do(Timeline timeline)
    {
        UnityEngine.Debug.Log("Discarding card " + cardToDiscard + " player: " + playerGui.PlayerModel.Name);
        GameObject objectToDiscard = playerGui.getCardInHand(cardToDiscard);
        objectToDiscardPosition = objectToDiscard.transform.position;
        objectToDiscardRotation = objectToDiscard.transform.rotation;
        playerGui.PlayerModel.RemoveCardInHand(cardToDiscard, true);
        game.actionCompleted = true;
    }

    public override float Act(bool qUndo = false)
    {
        playerGui.draw();
        Sequence sequence = DOTween.Sequence();
        GameObject cardToDiscardObject;
        if(cardToDiscard <24)
        {
            cardToDiscardObject= Object.Instantiate(gameGUI.CityCardPrefab, objectToDiscardPosition, objectToDiscardRotation, gameGUI.AnimationCanvas.transform);
            cardToDiscardObject.GetComponent<CityCardDisplay>().CityCardData = gameGUI.Cities[cardToDiscard].GetComponent<City>().city;
        }
        else
        {
            cardToDiscardObject = Object.Instantiate(gameGUI.EventCardPrefab, objectToDiscardPosition, objectToDiscardRotation, gameGUI.AnimationCanvas.transform);
            cardToDiscardObject.GetComponent<EventCardDisplay>().EventCardData = gameGUI.Events[cardToDiscard - 24];
        }
        sequence.Append(cardToDiscardObject.transform.DOMove(gameGUI.PlayerDeckDiscard.transform.position, ANIMATIONDURATION));
        sequence.Join(cardToDiscardObject.transform.DORotate(gameGUI.PlayerDeckDiscard.transform.eulerAngles, ANIMATIONDURATION));
        sequence.AppendCallback(() =>
        {
            gameGUI.drawBoard();
        });
        return sequence.Duration();
    }

    public override string GetLogInfo()
    {
        return $@" ""cardToDiscard"" : ""{cardToDiscard}""
                ";
    }
}