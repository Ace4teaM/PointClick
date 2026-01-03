using System;
using UnityEngine;

/// <summary>
/// Implémentation pour définir l'item de l'inventaire se trouvant sous le curseur
/// </summary>
public class HoverCursorFlagDialogChoice : MonoBehaviour, IHoverCursorFlag
{
    public int itemIndex = 0;
    public void Apply()
    {
        HoverCursorFlagStates.HoverFlag = GameData.ShowDialogChoices[itemIndex];
        HoverCursorFlagStates.HoverFlagType = HoverFlagType.DialogChoice;
        HoverCursorFlagStates.OnFlagChange();
    }
}
