using DG.Tweening;
using UnityEngine;
using static Game;
using static AnimationTemplates;

public class PCallToMobilizeCardPlayed : PlayerEvent
{
    private float ANIMATIONDURATION = 1f;

    private Player _eventActor;
    
    public PCallToMobilizeCardPlayed(Player playerThatTriggered) : base(playerThatTriggered)
    {
        _eventActor = playerThatTriggered;
    }

    public override void Do(Timeline timeline)
    {
        theGame.ChangeToInEvent(EventState.CALLTOMOBILIZE);
        _player.RemoveCardInHand(CALLTOMOBILIZEINDEX, true);
    }

    public override float Act(bool qUndo = false)
    {
        //GameObject cardToAddObject = _playerGui.AddPlayerCardToTransform(CALLTOMOBILIZEINDEX, gameGUI.AnimationCanvas.transform, false);
        Sequence sequence = HighlightCardAndMove(_playerGui.getCardInHand(CALLTOMOBILIZEINDEX), gameGUI.PlayerDeckDiscard.transform, 3f, ANIMATIONDURATION);
        /*sequence.onComplete += () =>
        {
            gameGUI.draw();
        };*/

        return base.Act(qUndo);
    }
    
    public override string GetLogInfo()
    {
        return $@" ""eventActor"" : ""{_eventActor}"",
                ";
    }
}