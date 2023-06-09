﻿using static GameGUI;
using static Game;
using UnityEngine;
using DG.Tweening;

internal class EIntensify : EngineEvent
{
    public float ANIMATIONDURATION = 2f / gui.AnimationTimingMultiplier;

    public EIntensify()
    {
    }

    public override void Do(Timeline timeline)
    {
        theGame.InfectionCardsDiscard.Shuffle();
        theGame.InfectionCards.AddRange(theGame.InfectionCardsDiscard);
        theGame.InfectionCardsDiscard.Clear();
    }

    public override float Act(bool qUndo = false)
    {
        gui.InfectionDiscard.DestroyChildrenImmediate();
        GameObject backCard = Object.Instantiate(gui.InfectionCardBackPrefab, gui.InfectionDiscard.transform.position, gui.PlayerDeck.transform.rotation, gui.InfectionDiscard.transform);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(backCard.transform.DOShakeRotation(ANIMATIONDURATION, new Vector3(0f, 0f, 3f), 10, 90, false));
        sequence.Append(backCard.transform.DOMove(gui.InfectionDeck.transform.position, ANIMATIONDURATION));
        sequence.AppendCallback(() =>
        {
            Object.Destroy(backCard);
            gui.draw();
            gui.EpidemicCardBoard.enabled = false;
        });

        return sequence.Duration();
    }
}