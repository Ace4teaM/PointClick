using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrentDialogText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = GameData.ShowDialog;
    }

    void Start()
    {
        GameData.OnDialogChanged += CurrentDialogText_OnDialogChanged;
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        GameData.OnDialogChanged -= CurrentDialogText_OnDialogChanged;
    }

    private void CurrentDialogText_OnDialogChanged(string text)
    {
        textMesh.text = text;
    }
}
