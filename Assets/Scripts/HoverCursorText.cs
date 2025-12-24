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
        textMesh.text = HoverCursorFlag.HoverFlag;
    }

    void Start()
    {
        HoverCursorFlag.OnFlagChanged += HoverCursorFlag_OnFlagChanged;
        GameData.OnSelectedItemChanged += GameData_OnSelectedItemChanged;
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        HoverCursorFlag.OnFlagChanged -= HoverCursorFlag_OnFlagChanged;
        GameData.OnSelectedItemChanged -= GameData_OnSelectedItemChanged;
    }

    void RefreshText()
    {
        var action = EnumExtensions.GetDescription(GameData.action);

        if (GameData.SelectedInventoryItem != InventoryItem.Empty)
        {
            if (HoverCursorFlag.HoverFlagType != HoverFlagType.None)
                textMesh.text = $"{action} {GameData.SelectedInventoryItem.label} sur {HoverCursorFlag.HoverFlag}";
            else
                textMesh.text = $"{action} {GameData.SelectedInventoryItem.label} sur ...";
        }
        else
        {
            if (HoverCursorFlag.HoverFlagType != HoverFlagType.None)
                textMesh.text = $"{action} {HoverCursorFlag.HoverFlag}";
            else
                textMesh.text = action;
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
