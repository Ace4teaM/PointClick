using System;
using UnityEngine;

/// <summary>
/// Données statiques non persistantes du jeu
/// </summary>
public static class GameData
{
    public static ActionType action;

    public static event Action<ActionType> OnActionChanged;

    internal static void OnActionChange()
    {
        OnActionChanged?.Invoke(action);
    }
}
