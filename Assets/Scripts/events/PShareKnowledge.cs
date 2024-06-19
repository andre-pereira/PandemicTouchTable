using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Timeline.TimelineAsset;

public class PShareKnowledge : PlayerEvent
{
    PlayerGUI playerFrom, playerTo;
    float ANIMATIONDURATION = 1f/ GameGUI.gui.AnimationTimingMultiplier;
    private int cityID;
    Vector3 initialPosition;
    Quaternion initialRotation;
    CityCard cardData;

    public PShareKnowledge(PlayerGUI playerFrom, PlayerGUI playerTo) : base(Game.theGame.CurrentPlayer)
    {
        this.playerFrom = playerFrom;
        this.playerTo = playerTo;
        cityID = playerFrom.PlayerModel.GetCurrentCity();
        CityCardDisplay cityCard = playerFrom.getCardInHand(cityID).GetComponent<CityCardDisplay>();
        initialPosition = cityCard.transform.position;
        initialRotation = cityCard.transform.rotation;
        cardData = cityCard.CityCardData;
    }

    public override void Do(Timeline timeline)
    {
        playerFrom.PlayerModel.RemoveCardInHand(cityID);
        playerTo.PlayerModel.AddCardToHand(cityID);
        playerTo.UpdateCardsState(CardGUIStates.None);
        playerFrom.UpdateCardsState(CardGUIStates.None);
        _player.DecreaseActionsRemaining(1);
    }

    public override float Act(bool qUndo = false)
    {
        playerFrom.draw();
        GameObject cityCardCopy = Object.Instantiate(gameGUI.CityCardPrefab, initialPosition, initialRotation, gameGUI.AnimationCanvas.transform);
        CityCardDisplay cityCardCopyDisplay = cityCardCopy.GetComponent<CityCardDisplay>();
        Sequence sequence = DOTween.Sequence();
        cityCardCopyDisplay.CityCardData = cardData;
        GameObject toMoveTo = playerTo.getFirstCardInHand();
        if (toMoveTo != null)
            toMoveTo = playerTo.PlayerCards;
        sequence.Append(cityCardCopy.transform.DOMove(toMoveTo.transform.position, ANIMATIONDURATION));
        sequence.Join(cityCardCopy.transform.DORotate(toMoveTo.transform.rotation.eulerAngles, ANIMATIONDURATION));
        sequence.AppendCallback(() => {
            Object.Destroy(cityCardCopy);
            playerTo.draw();
        });
        
        return sequence.Duration();
    }

    public override string GetLogInfo()
    {
        return $@" ""cityId"" : {cityID},
                    ""playerFrom"" : ""{playerFrom.PlayerModel.Role}"",
                    ""playerTo"" : ""{playerTo.PlayerModel.Role}""
                ";
    }
}