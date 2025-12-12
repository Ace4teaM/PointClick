using System;

/// <summary>
/// Données statiques non persistantes du jeu
/// </summary>
public static class GameData
{
    /// <summary>
    /// Action actuelle du curseur
    /// </summary>
    public static ActionType action;

    /// <summary>
    /// Evénement si action change
    /// </summary>
    public static event Action<ActionType> OnActionChanged;

    /// <summary>
    /// Evénement click
    /// </summary>
    public static event Action InputClickEvent;

    /// <summary>
    /// Evénement dialogue
    /// </summary>
    public static event Action<string> OnDialogChanged;

    /// <summary>
    /// Evénement animation
    /// </summary>
    public static event Action<string> OnAnimationChanged;

    /// <summary>
    /// Nom de la scène utilisée pour la prochaine transition
    /// </summary>
    public static string TransitionScene = "CircleTransition";
    /// <summary>
    /// Nom de la scène actuellement chargée pour l'UI
    /// </summary>
    public static string CurrentSceneGame;
    /// <summary>
    /// Nom de la scène actuellement chargée pour l'environnement
    /// </summary>
    public static string CurrentSceneUI;
    /// <summary>
    /// Texte du dialogue en cours
    /// </summary>
    public static string ShowDialog;
    /// <summary>
    /// Nom de l'aniamtion en cours
    /// </summary>
    public static string ShowAnimation;

    internal static void OnDialogChange()
    {
        OnDialogChanged?.Invoke(ShowDialog);
    }

    internal static void OnAnimationChange()
    {
        OnAnimationChanged?.Invoke(ShowAnimation);
    }

    internal static void OnActionChange()
    {
        OnActionChanged?.Invoke(action);
    }

    internal static void OnInputClick()
    {
        InputClickEvent?.Invoke();
    }
}
