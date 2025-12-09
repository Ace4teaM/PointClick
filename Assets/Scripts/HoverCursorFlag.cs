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
public class HoverCursorFlag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static HoverFlagType HoverFlagType = HoverFlagType.None;
    public static string HoverFlag = string.Empty;

    public HoverFlagType flagType = HoverFlagType.None;
    public string flag = string.Empty;

    public static event Action<HoverFlagType, string> OnFlagChanged;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverFlag = flag;
        HoverFlagType = flagType;
        OnFlagChanged?.Invoke(HoverFlagType, HoverFlag);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverFlag = string.Empty;
        HoverFlagType = HoverFlagType.None;
        OnFlagChanged?.Invoke(HoverFlagType, HoverFlag);
    }
}
