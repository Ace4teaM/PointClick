using System;
using UnityEngine;

public abstract class AnimationAsset : MonoBehaviour
{
    public abstract void OnAnimate(Animations anim, string animationName);
}
