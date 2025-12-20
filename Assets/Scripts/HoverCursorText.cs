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
        if (GameData.SelectedInventoryItem != InventoryItem.Empty)
        {
            if(HoverCursorFlag.HoverFlagType != HoverFlagType.None)
                textMesh.text = $"{GameData.action} {GameData.SelectedInventoryItem.label} avec {HoverCursorFlag.HoverFlag}";
            else
                textMesh.text = $"{GameData.action} {GameData.SelectedInventoryItem.label} avec ...";
        }
        else
            textMesh.text = HoverCursorFlag.HoverFlag;
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
