using DG.Tweening;
using static Game;
using static AnimationTemplates;

public class PResourcePlanningCardPlayed : PlayerEvent
{
    public PResourcePlanningCardPlayed(Player player) : base(player)
    {
    }

    public override void Do(Timeline timeline)
    {
        _player.playerGui.ChangeToInEvent(EventState.NOTINEVENT);
        _player.RemoveCardInHand(RESOURCEPLANNINGINDEX, true);
    }

    public override float Act(bool qUndo = false)
    {
        Sequence sequence = HighlightCardAndMove(_playerGui.getCardInHand(RESOURCEPLANNINGINDEX), gameGUI.PlayerDeckDiscard.transform, 3f, ANIMATIONDURATION);
        sequence.onComplete += () =>
        {
            gameGUI.draw();

        };

        return base.Act(qUndo);
    }
    
}