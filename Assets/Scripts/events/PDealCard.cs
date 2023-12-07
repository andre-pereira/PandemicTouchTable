using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameGUI;
using static Game;
using static AnimationTemplates;

public class PDealCard : PlayerEvent
{
    private float ANIMATIONDURATION = 0.5f / GameGUI.gui.AnimationTimingMultiplier;
    int cardToAdd = -1;
    private bool epidemicPopped = false;

    public PDealCard(Player player): base(player){}

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
            _player.AddCardToHand(cardToAdd);
        }
        theGame.PlayerCardsDrawn++;
        theGame.actionCompleted = true;
    }

    public override float Act(bool qUndo = false)
    {
        if (epidemicPopped)
            return 0f;

        GameObject card = game.AddPlayerCardToTransform(cardToAdd, gameGUI.AnimationCanvas.transform, false, _playerGui, gameGUI.PlayerDeck.transform);
        gameGUI.drawBoard();
        Sequence sequence = HighlightCardAndMove(card, _playerGui.roleCard.transform, 3f, ANIMATIONDURATION);
        sequence.onComplete += () =>
        {
            Object.Destroy(card);
            _playerGui.draw();
        };
        sequence.Play();
        return sequence.Duration();
    }


}
