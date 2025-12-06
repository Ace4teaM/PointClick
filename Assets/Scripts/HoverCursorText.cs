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
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        HoverCursorFlag.OnFlagChanged -= HoverCursorFlag_OnFlagChanged;
    }

    private void HoverCursorFlag_OnFlagChanged(HoverFlagType type, string flag)
    {
        if(GameData.action != ActionType.Move)
            textMesh.text = flag;
    }
}
