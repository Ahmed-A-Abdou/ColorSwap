using UnityEngine;
using DG.Tweening;
public static class TweenHelper
{
    public static Tween MoveToPosition(Transform target, Vector3 destination, float duration = 0.3f)
    {
        return target.DOMove(destination, duration).SetEase(Ease.OutBack);
    }

    public static void MoveUIToPosition(RectTransform target, Vector2 destination, float duration = 0.5f)
    {
        target.DOAnchorPos(destination, duration).SetEase(Ease.OutBack);
    }
}
