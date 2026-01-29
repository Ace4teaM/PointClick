using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrentDialogText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    public DialogColors dialogColors;

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

    void SetDialog(string text)
    {
        string nom = "default";
        var i = text.IndexOf(':');
        if(i != -1 && i > 0)
        {
            nom = text.Substring(0, i).Trim();
        }

        var color = dialogColors.GetColor(nom);
        if (color != null)
        {
            textMesh.color = color.color;
            textMesh.outlineColor = color.outline;
        }
        else
        {
            textMesh.color = Color.white;
            textMesh.outlineColor = Color.black;
        }

        textMesh.text = text;
    }

    void OnDestroy()
    {
        GameData.OnDialogChanged -= CurrentDialogText_OnDialogChanged;
    }

    private void CurrentDialogText_OnDialogChanged(string text)
    {
        SetDialog(text);
    }
}
