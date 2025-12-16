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

    private void OnEnable()
    {
        // S'abonner à l'event global
        GameData.OnDialogChanged += CurrentDialogText_OnDialogChanged;
    }

    private void OnDisable()
    {
        // Se désabonner pour éviter les fuites
        GameData.OnDialogChanged -= CurrentDialogText_OnDialogChanged;
    }

    void Start()
    {
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
