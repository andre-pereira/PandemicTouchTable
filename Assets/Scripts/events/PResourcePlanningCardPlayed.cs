using DG.Tweening;
using static Game;
using static AnimationTemplates;
using System.Collections.Generic;
using System;

public class PResourcePlanningCardPlayed : PlayerEvent
{
    public new float ANIMATIONDURATION = 1f;

    public List<int> cardsToSort = new List<int>();

    public PResourcePlanningCardPlayed(Player player) : base(player)
    {
    }

    public override void Do(Timeline timeline)
    {
        int numberOfCardsInDeck = theGame.PlayerCards.Count;
        if (numberOfCardsInDeck > 0)
        {
            _player.playerGui.ChangeToInEvent(EventState.RESOURCEPLANNING, false);
            _player.RemoveCardInHand(RESOURCEPLANNINGINDEX, true);

            for (int i = 0; i < Math.Min(4, numberOfCardsInDeck); i++)
            {
                cardsToSort.Add(theGame.PlayerCards.Pop());
            }
        }

    }

    public override float Act(bool qUndo = false)
    {
        _playerGui.EnableContextButtons(false, false, false, false, false, false);
        Sequence sequence = HighlightCardAndMove(_playerGui.getCardInHand(RESOURCEPLANNINGINDEX), gameGUI.PlayerDeckDiscard.transform, 3f, ANIMATIONDURATION);
        sequence.onComplete += () =>
        {
            for (int i = 0; i < cardsToSort.Count; i++)
            {
                _player.playerGui.ResourcePlanningEventCardsIDs.Add(cardsToSort[i]);
            }
            _player.playerGui.ResourcePlanningEventCardSelected = _player.playerGui.ResourcePlanningEventCardsIDs[0];
            _player.playerGui.ActionsContainer.SetActive(false);

            _player.playerGui.draw();
            gameGUI.draw();
            
        };

        return base.Act(qUndo);
    }

    public override string GetLogInfo()
    {
        string cardIds = string.Join(", ", cardsToSort);
        return $@" ""cardsToSort"" : [{cardIds}]";
    }
}