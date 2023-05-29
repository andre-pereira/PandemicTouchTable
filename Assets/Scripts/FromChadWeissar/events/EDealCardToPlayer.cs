using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EDealCardToPlayer : EngineEvent
{
    private const float scaleToCenterScale = 3f;
    private const float scaleToCenterDuration = 0.01f;

    GameGUI gui = GameGUI.gui;
    Game game = Game.theGame;
    Player player = null;
    PlayerGUI playerGui = null;
    private bool waitForEnd;
    int cardToAdd = -1;
    private bool epidemicPopped = false;

    public EDealCardToPlayer(Player player, bool waitForEnd)
    {
        QUndoable = false;
        this.player = player;
        playerGui = GameGUI.playerPadForPlayer(player);
        this.waitForEnd = waitForEnd;
    }

    public override void Do(Timeline timeline)
    {
        cardToAdd = game.PlayerCards.Pop();
        if (cardToAdd == 28)
        {
            Timeline.theTimeline.addEvent(new EEpidemic());
            epidemicPopped = true;
        }
        else
        {
            player.AddCardToHand(cardToAdd);
        }
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
        sequence.Append(cardToAddObject.transform.DOShakeRotation(scaleToCenterDuration/2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), scaleToCenterDuration)).
            Join(cardToAddObject.transform.DOMove(new Vector3(0, 0, 0), scaleToCenterDuration));
        sequence.AppendInterval(scaleToCenterDuration);
        sequence.Append(cardToAddObject.transform.DOScale(new Vector3(1f, 1f, 1f), scaleToCenterDuration)).
            Join(cardToAddObject.transform.DORotate(playerGui.transform.rotation.eulerAngles, scaleToCenterDuration)).
            Join(cardToAddObject.transform.DOMove(playerGui.PlayerCards.transform.position, scaleToCenterDuration)).
            OnComplete(() => {
                Object.Destroy(cardToAddObject);
                playerGui.draw();
            });
        sequence.Play();


        //Texture frontTexture = cardToAddObject.GetComponent<Image>().image;

        //cardToAddObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), flipDuration)
        //    .OnComplete(() =>
        //    {
        //        cardToAddObject.transform.DORotate(new Vector3(0, 90, 0), flipDuration / 2).WaitForCompletion();
        //        cardToAddObject.GetComponent<Image>().image = isShowingFront ? gui.PlayerCardBack : frontTexture;
        //        isShowingFront = !isShowingFront;
        //       cardToAddObject.transform.DORotate(new Vector3(0, 0, 0), flipDuration / 2).WaitForCompletion();

        //        // Shrink and move the card to hand after flip
        //        cardToAddObject.transform.DOScale(Vector3.one, flipDuration);
        //        cardToAddObject.transform.DOMove(playerGui.PlayerCards.transform.position, flipDuration)
        //            .SetEase(Ease.InOutQuad);
        //    });
        if(waitForEnd)
            return sequence.Duration();
        else return 0f;
    }

}
