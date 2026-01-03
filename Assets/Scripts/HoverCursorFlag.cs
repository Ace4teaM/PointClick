using System;
using UnityEngine;

/// <summary>
/// Implémentation pour définir l'objet de la scène se trouvant sous le curseur
/// </summary>
public class HoverCursorFlag : MonoBehaviour, IHoverCursorFlag
{
    public HoverFlagType flagType = HoverFlagType.None;
    public string flag = string.Empty;

    public void Apply()
    {
        HoverCursorFlagStates.HoverFlag = flag;
        HoverCursorFlagStates.HoverFlagType = flagType;
        HoverCursorFlagStates.OnFlagChange();
    }
}
