using Description = System.ComponentModel.DescriptionAttribute;

[System.Serializable]
public enum ActionType
{
    [Description("None")]
    None,
    [Description("Valider")]
    Validate,
    [Description("Déplacer vers")]
    Move,
    [Description("Inspecter")]
    Inspect,
    [Description("Parler à")]
    Talk,
    [Description("Utiliser")]
    Activate,
    [Description("Intéragir avec")]
    Interact,
    [Description("Choisir")]
    Choice
}