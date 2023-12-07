using DG.Tweening;
using UnityEngine;
using static GameGUI;
using static Game;

//Create a new static class that holds useful animation templates
public static class AnimationTemplates
{
    public static Sequence HighlightCardAndMove(GameObject objectToAnimate, Transform finalLocation, float scaleToCenterScale, float animationDuration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(objectToAnimate.transform.DOShakeRotation(animationDuration / 2, new Vector3(0f, 0f, scaleToCenterScale), 10, 90, false));
        sequence.Append(objectToAnimate.transform.DOScale(new Vector3(scaleToCenterScale, scaleToCenterScale, 1f), animationDuration)).
            Join(objectToAnimate.transform.DOMove(new Vector3(0, 0, 0), animationDuration));
        sequence.AppendInterval(animationDuration);
        sequence.Append(objectToAnimate.transform.DOScale(new Vector3(1f, 1f, 1f), animationDuration)).
            Join(objectToAnimate.transform.DORotate(finalLocation.rotation.eulerAngles, animationDuration)).
            Join(objectToAnimate.transform.DOMove(finalLocation.position, animationDuration));
        return sequence;
    }
}
