using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameGUI;
using static Game;

public class EDealCardToPlayer : EngineEvent
{
    private const float scaleToCenterScale = 3f;
    private float ANIMATIONDURATION = 0.5f / gui.AnimationTimingMultiplier;

    Player player = null;
    PlayerGUI playerGui = null;
    int cardToAdd = -1;
    private bool epidemicPopped = false;

    public EDealCardToPlayer(Player player)
    {
        QUndoable = false;
        this.player = player;
        playerGui = playerPadForPlayer(player);
    }

    public override void Do(Timeline timeline)
    {
        cardToAdd = theGame.PlayerCards.Pop();
        if (cardToAdd == 28)
        {
            Timeline.theTimeline.addEvent(new EEpidemicInitiate());
            epidemicPopped = true;
        }
        else
        {
            player.AddCardToHand(cardToAdd);
        }
        theGame.PlayerCardsDrawn++;
        theGame.actionCompleted = true;
    }

    public override float Act(bool qUndo = false)
    {
        if(epidemicPopped)
            return 0f;

        GameObject cardToAddObject = playerGui.AddPlayerCardToTransform(cardToAdd, gui.AnimationCanvas.transform,false);
        cardToAddObject.transform.position = gui.PlayerDeck.transform.position;
        cardToAddObject.transform.rotation = gui.PlayerDeck.transform.rotation;
        gui.drawBoard();
        Sequence sequence = DOTween.Sequence();

        sequence.Append(cardToAddObject.transform.DOShakeRotation(ANIMATIONDURATION / 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), ANIMATIONDURATION)).
            Join(cardToAddObject.transform.DOMove(new Vector3(0, 0, 0), ANIMATIONDURATION));
        sequence.AppendInterval(ANIMATIONDURATION);
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(1f, 1f, 1f), ANIMATIONDURATION)).
            Join(cardToAddObject.transform.DORotate(playerGui.transform.rotation.eulerAngles, ANIMATIONDURATION)).
            Join(cardToAddObject.transform.DOMove(playerGui.roleCard.transform.position, ANIMATIONDURATION)).
            OnComplete(() => {
                Object.Destroy(cardToAddObject);
                playerGui.draw();
            });
        sequence.Play();
        
        return sequence.Duration();
    }

}
