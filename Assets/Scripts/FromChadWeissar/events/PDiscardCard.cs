using DG.Tweening;
using System.Diagnostics;
using UnityEngine;

internal class PDiscardCard : PlayerEvent
{
    private const float ANIMATIONDURATION = 1f;
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
        playerGui.PlayerModel.RemoveCardInHand(cardToDiscard);
        game.PlayerCardsDiscard.Add(cardToDiscard);
        game.actionCompleted = true;
    }

    public override float Act(bool qUndo = false)
    {
        playerGui.draw();
        Sequence sequence = DOTween.Sequence();
        GameObject cardToDiscardObject;
        if(cardToDiscard <24)
        {
            cardToDiscardObject= Object.Instantiate(gui.CityCardPrefab, objectToDiscardPosition, objectToDiscardRotation, gui.AnimationCanvas.transform);
            cardToDiscardObject.GetComponent<CityCardDisplay>().CityCardData = gui.Cities[cardToDiscard].GetComponent<City>().city;
        }
        else
        {
            cardToDiscardObject = Object.Instantiate(gui.EventCardPrefab, objectToDiscardPosition, objectToDiscardRotation, gui.AnimationCanvas.transform);
            cardToDiscardObject.GetComponent<EventCardDisplay>().EventCardData = gui.Events[cardToDiscard - 24];
        }
        sequence.Append(cardToDiscardObject.transform.DOMove(gui.PlayerDeckDiscard.transform.position, ANIMATIONDURATION));
        sequence.Join(cardToDiscardObject.transform.DORotate(gui.PlayerDeckDiscard.transform.eulerAngles, ANIMATIONDURATION));
        sequence.AppendCallback(() =>
        {
            gui.drawBoard();
        });
        return sequence.Duration();
    }
}