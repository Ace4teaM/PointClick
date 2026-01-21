using Description = System.ComponentModel.DescriptionAttribute;

[System.Serializable]
public enum UiType
{
    [Description("None")]
    None,
    [Description("Jeu")]
    Game,
    [Description("Recherche")]
    Search,
    [Description("Cinématique")]
    CutScene
}