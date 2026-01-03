using System;
using UnityEngine;

/// <summary>
/// Implémentation pour définir l'item de l'inventaire se trouvant sous le curseur
/// </summary>
public class HoverCursorFlagItem : MonoBehaviour, IHoverCursorFlag
{
    public int itemIndex = 0;
    public void Apply()
    {
        HoverCursorFlagStates.HoverFlag = GameData.InventoryItems[itemIndex].label;
        HoverCursorFlagStates.HoverFlagType = HoverFlagType.GameItem;
        HoverCursorFlagStates.OnFlagChange();
    }
}
