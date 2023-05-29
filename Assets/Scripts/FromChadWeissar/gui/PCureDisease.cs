using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

internal class PCureDisease : PlayerEvent
{
    private ENUMS.VirusName virusName;
    private List<int> selectedCards;
    private const float ANIMATIONDURATION = 1f;
    private Vector3[] originalCardPositions;
    private Quaternion[] originalCardRotations;

    public PCureDisease(ENUMS.VirusName virusName, List<int> selectedCards): base(Game.theGame.CurrentPlayer)
    {
        this.selectedCards = new List<int>(selectedCards);
        originalCardPositions = new Vector3[selectedCards.Count];
        originalCardRotations = new Quaternion[selectedCards.Count];
        this.virusName = virusName;
        for (int i = 0; i < selectedCards.Count; i++)
        {
            originalCardPositions[i] = _playerGui.getCardInHand(selectedCards[i]).transform.position;
            originalCardRotations[i] = _playerGui.getCardInHand(selectedCards[i]).transform.rotation;
        }
    }

    public override void Do(Timeline timeline)
    {
        for (int i = 0; i < selectedCards.Count; i++)
        {
            _player.RemoveCityCardInHand(selectedCards[i]);
            game.PlayerCardsDiscard.Add(selectedCards[i]);
        }
        switch (virusName)
        {
            case ENUMS.VirusName.Blue:
                game.BlueCure = true;
                break;
            case ENUMS.VirusName.Red:
                game.RedCure = true;
                break;
            case ENUMS.VirusName.Yellow:
                game.YellowCure = true;
                break;
        }
    }

    public override float Act(bool qUndo = false)
    {
        Sequence sequence = DOTween.Sequence();
        _playerGui.draw();
        for (int i = 0; i < selectedCards.Count; i++)
        {
            GameObject cardToAddObject = _playerGui.AddPlayerCardToTransform(selectedCards[i], gui.PlayerDeckDiscard.transform, false);
            cardToAddObject.transform.position = originalCardPositions[i];
            cardToAddObject.transform.rotation = originalCardRotations[i];
            sequence.Join(cardToAddObject.transform.DOMove(gui.PlayerDeckDiscard.transform.position, ANIMATIONDURATION));
            sequence.Join(cardToAddObject.transform.DORotate(gui.PlayerDeckDiscard.transform.eulerAngles, ANIMATIONDURATION));
        }
        sequence.Append(gui.VialTokens[(int)virusName].transform.DOMove(gui.VialTokensTransforms[(int)virusName].transform.position, ANIMATIONDURATION).OnComplete(() =>
            {
                gui.drawBoard();
            }));
        return sequence.Duration();
    }
}