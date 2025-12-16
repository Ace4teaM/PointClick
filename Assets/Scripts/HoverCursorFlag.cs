using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum HoverFlagType
{
    None,
    GameObject,
    UI
}

/// <summary>
/// Implémentation globale pour définir l'objet se trouvant sous le curseur
/// </summary>
public class HoverCursorFlag : MonoBehaviour
{
    public static HoverFlagType HoverFlagType = HoverFlagType.None;
    public static string HoverFlag = string.Empty;

    public HoverFlagType flagType = HoverFlagType.None;
    public string flag = string.Empty;

    public static event Action<HoverFlagType, string> OnFlagChanged;

    internal void Apply()
    {
        HoverFlag = flag;
        HoverFlagType = flagType;
        OnFlagChanged?.Invoke(HoverFlagType, HoverFlag);
    }

    internal static void UnApply()
    {
        HoverFlag = string.Empty;
        HoverFlagType = HoverFlagType.None;
        OnFlagChanged?.Invoke(HoverFlagType, HoverFlag);
    }
}
