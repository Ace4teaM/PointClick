using DG.Tweening;
using UnityEngine;

public class AnimPingPongScale : MonoBehaviour
{
    public float maxScale = 1.2f;
    public float duration = 0.5f;
    void Start()
    {
        transform
            .DOScale(Vector3.one * maxScale, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
