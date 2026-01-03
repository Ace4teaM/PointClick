using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum HoverFlagType
{
    None,
    GameObject,
    UI,
    GameItem,
    DialogChoice
}

/// <summary>
/// Implémentation globale pour définir l'objet se trouvant sous le curseur
/// </summary>
public static class HoverCursorFlagStates
{
    public static HoverFlagType HoverFlagType = HoverFlagType.None;
    public static string HoverFlag = string.Empty;

    public static event Action<HoverFlagType, string> OnFlagChanged;

    internal static void OnFlagChange()
    {
        OnFlagChanged?.Invoke(HoverFlagType, HoverFlag);
    }

    internal static void Apply(HoverFlagType flagType, string flag)
    {
        HoverFlag = flag;
        HoverFlagType = flagType;
        OnFlagChange();
    }

    internal static void UnApply()
    {
        HoverFlag = string.Empty;
        HoverFlagType = HoverFlagType.None;
        OnFlagChange();
    }
}

/// <summary>
/// Interface générale pour définir l'objet se trouvant sous le curseur
/// </summary>
public interface IHoverCursorFlag
{
    void Apply();
}