using UnityEngine;
using UnityEngine.EventSystems;

public enum HoverFlagType
{
    None,
    GameObject,
    UI
}

public class HoverCursorFlag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static HoverFlagType HoverFlagType = HoverFlagType.None;
    public static string HoverFlag = string.Empty;

    public HoverFlagType flagType = HoverFlagType.None;
    public string flag = string.Empty;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverFlag = flag;
        HoverFlagType = flagType;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverFlag = string.Empty;
        HoverFlagType = HoverFlagType.None;
    }
}
