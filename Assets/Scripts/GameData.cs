using System;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static ActionType action;

    public static event Action<ActionType> OnActionChanged;


    public void EnableMove()
    {
        action = ActionType.Move;
        OnActionChanged?.Invoke(action);
    }

    public void EnableInspect()
    {
        action = ActionType.Inspect;
        OnActionChanged?.Invoke(action);
    }

    public void EnableTalk()
    {
        action = ActionType.Talk;
        OnActionChanged?.Invoke(action);
    }

    public void EnableActivate()
    {
        action = ActionType.Activate;
        OnActionChanged?.Invoke(action);
    }
}
