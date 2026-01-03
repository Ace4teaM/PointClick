using TMPro;
using UnityEngine;

/// <summary>
/// Copie dans le texte de l'objet le flag de l'objet se trouvant sous le curseur
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class HoverCursorText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = HoverCursorFlagStates.HoverFlag;
    }

    void Start()
    {
        HoverCursorFlagStates.OnFlagChanged += HoverCursorFlag_OnFlagChanged;
        GameData.OnSelectedItemChanged += GameData_OnSelectedItemChanged;
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        HoverCursorFlagStates.OnFlagChanged -= HoverCursorFlag_OnFlagChanged;
        GameData.OnSelectedItemChanged -= GameData_OnSelectedItemChanged;
    }

    void RefreshText()
    {
        var action = EnumExtensions.GetDescription(GameData.action);

        if (GameData.SelectedInventoryItem != InventoryItem.Empty)
        {
            if (HoverCursorFlagStates.HoverFlagType == HoverFlagType.GameObject)
                textMesh.text = $"{action} {GameData.SelectedInventoryItem.label} sur {HoverCursorFlagStates.HoverFlag}";
            else
                textMesh.text = $"{action} {GameData.SelectedInventoryItem.label} sur ...";
        }
        else
        {
            if (HoverCursorFlagStates.HoverFlagType == HoverFlagType.GameObject)
                textMesh.text = $"{action} {HoverCursorFlagStates.HoverFlag}";
            else if (HoverCursorFlagStates.HoverFlagType == HoverFlagType.GameItem && GameData.action == ActionType.Activate)
                textMesh.text = $"{action} {HoverCursorFlagStates.HoverFlag}";
            else if (HoverCursorFlagStates.HoverFlagType == HoverFlagType.GameItem)
                textMesh.text = $"{HoverCursorFlagStates.HoverFlag}";
            else if (HoverCursorFlagStates.HoverFlagType == HoverFlagType.UI)
                textMesh.text = $"{HoverCursorFlagStates.HoverFlag}";
            else
                textMesh.text = $"{action} ...";
        }
    }

    private void HoverCursorFlag_OnFlagChanged(HoverFlagType type, string flag)
    {
        RefreshText();
    }

    private void GameData_OnSelectedItemChanged(InventoryItem obj)
    {
        RefreshText();
    }
}
