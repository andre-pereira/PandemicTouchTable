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

    public PCureDisease(List<int> selectedCards): base(Game.theGame.CurrentPlayer)
    {
        this.selectedCards = new List<int>(selectedCards);
        originalCardPositions = new Vector3[selectedCards.Count];
        originalCardRotations = new Quaternion[selectedCards.Count];

        int numRed = 0;
        int numYellow = 0;
        int numBlue = 0;

        for (int i = 0; i < selectedCards.Count; i++)
        {
            originalCardPositions[i] = _playerGui.getCardInHand(selectedCards[i]).transform.position;
            originalCardRotations[i] = _playerGui.getCardInHand(selectedCards[i]).transform.rotation;
            switch (game.Cities[selectedCards[i]].city.virusInfo.virusName)
            {
                case ENUMS.VirusName.Blue:
                    numBlue++;
                    if(numBlue > 2)
                        virusName = ENUMS.VirusName.Blue;
                    break;
                case ENUMS.VirusName.Red:
                    numRed++;
                    if (numRed > 2)
                        virusName = ENUMS.VirusName.Red;
                    break;
                case ENUMS.VirusName.Yellow:
                    numYellow++;
                    if (numYellow > 2)
                        virusName = ENUMS.VirusName.Yellow;
                    break;
            }
        }
    }

    public override void Do(Timeline timeline)
    {
        for (int i = 0; i < selectedCards.Count; i++)
        {
            _player.RemoveCardInHand(selectedCards[i], true);
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

        _player.DecreaseActionsRemaining(1);
        _playerGui.ActionSelected = ActionTypes.None;
        // Check if all diseases are cured and if so, end the game

        if (game.BlueCure && game.RedCure && game.YellowCure)
        {
            Timeline.theTimeline.clearPendingEvents();
            timeline.addEvent(new EGameOver(ENUMS.GameOverReasons.PlayersWon));
        }
    }

    public override float Act(bool qUndo = false)
    {
        Sequence sequence = DOTween.Sequence();
        _playerGui.draw();
        for (int i = 0; i < selectedCards.Count; i++)
        {
            GameObject cardToAddObject = game.AddPlayerCardToTransform(selectedCards[i], gameGUI.PlayerDeckDiscard.transform, false, _playerGui);
            cardToAddObject.transform.position = originalCardPositions[i];
            cardToAddObject.transform.rotation = originalCardRotations[i];
            sequence.Join(cardToAddObject.transform.DOMove(gameGUI.PlayerDeckDiscard.transform.position, ANIMATIONDURATION));
            sequence.Join(cardToAddObject.transform.DORotate(gameGUI.PlayerDeckDiscard.transform.eulerAngles, ANIMATIONDURATION));
        }
        sequence.Append(gameGUI.VialTokens[(int)virusName].transform.DOMove(gameGUI.VialTokensTransforms[(int)virusName].transform.position, ANIMATIONDURATION).OnComplete(() =>
            {
                gameGUI.drawBoard();
            }));
        return sequence.Duration();
    }
}