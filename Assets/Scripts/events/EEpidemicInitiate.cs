using DG.Tweening;
using UnityEngine;
using static Game;
using static GameGUI;

public class EEpidemicInitiate : EngineEvent
{
    private float AnimationDuration = 1f / gui.AnimationTimingMultiplier;
    private const float scaleToCenterScale = 3f;

    public EEpidemicInitiate()
    {

    }

    public override void Do(Timeline timeline)
    {
        theGame.setCurrentGameState(GameState.EPIDEMIC);
    }

    public override float Act(bool qUndo = false)
    {
        GameObject epidemicCard = Object.Instantiate(gui.EpidemicCardPrefab, gui.PlayerDeck.transform.position, gui.PlayerDeck.transform.rotation, gui.AnimationCanvas.transform);
        
        gui.drawBoard();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(epidemicCard.transform.DOShakeRotation(AnimationDuration / 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        sequence.Append(epidemicCard.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), AnimationDuration)).
            Join(epidemicCard.transform.DOMove(new Vector3(0, 0, 0), AnimationDuration));
        sequence.AppendInterval(AnimationDuration);
        sequence.Append(epidemicCard.transform.DOScale(new Vector3(1f, 1f, 1f), AnimationDuration)).
            Join(epidemicCard.transform.DORotate(gui.EpidemicCardBoard.transform.rotation.eulerAngles, AnimationDuration)).
            Join(epidemicCard.transform.DOMove(gui.EpidemicCardBoard.transform.position, AnimationDuration)).
            OnComplete(() => {
                Object.Destroy(epidemicCard);
                gui.EpidemicCardBoard.enabled = true;
            });
        sequence.Play();
        
        return sequence.Duration();
    }
}