using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrentChoiceText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    public int choiceIndex;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = GameData.ShowDialog;
        GameData.OnChoicesChanged += CurrentDialogText_OnChoicesChanged;
        SetText(GameData.ShowDialogChoices[choiceIndex]);
    }
    void OnDestroy()
    {
        GameData.OnChoicesChanged -= CurrentDialogText_OnChoicesChanged;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        // S'abonner à l'event global
        GameData.OnChoicesChanged += CurrentDialogText_OnChoicesChanged;
    }

    private void OnDisable()
    {
        // Se désabonner pour éviter les fuites
        GameData.OnChoicesChanged -= CurrentDialogText_OnChoicesChanged;
    }
#endif

    void SetText(string text)
    {
        textMesh.text = text;
    }

    private void CurrentDialogText_OnChoicesChanged(string[] choices)
    {
        SetText(choices[choiceIndex]);
    }
}
