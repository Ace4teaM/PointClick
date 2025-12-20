using System;

/// <summary>
/// Données statiques non persistantes du jeu
/// </summary>
public static class GameData
{
    //Item sélectionné
    public static InventoryItem SelectedInventoryItem = InventoryItem.Empty;

    // Items de l'inventaire
    public static InventoryItem[] InventoryItems = new InventoryItem[4]{
        InventoryItem.Empty,
        InventoryItem.Empty,
        InventoryItem.Empty,
        InventoryItem.Empty
    };

    /// <summary>
    /// Action actuelle du curseur
    /// </summary>
    public static ActionType action;

    /// <summary>
    /// Le choix de l'action en cours a changé
    /// </summary>
    public static event Action<ActionType> OnActionChanged;

    /// <summary>
    /// Evénement click de la souris
    /// </summary>
    public static event Action InputClickEvent;

    /// <summary>
    /// Le curseur a bougé
    /// </summary>
    public static event Action InputMoveEvent;

    /// <summary>
    /// Le texte du dialogue en cours a changé
    /// </summary>
    public static event Action<string> OnDialogChanged;

    /// <summary>
    /// Evénement animation
    /// </summary>
    public static event Action<string> OnAnimationChanged;

    /// <summary>
    /// Les items dans l'inventaire ont changés
    /// </summary>
    public static event Action<InventoryItem[]> OnInventoryChanged;

    /// <summary>
    /// L'item sélectionné a changé
    /// </summary>
    public static event Action<InventoryItem> OnSelectedItemChanged;

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

    internal static void OnSelectedItemChange()
    {
        OnSelectedItemChanged?.Invoke(SelectedInventoryItem);
    }

    internal static void OnInventoryChange()
    {
        OnInventoryChanged?.Invoke(InventoryItems);
    }

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
    internal static void OnInputMove()
    {
        InputMoveEvent?.Invoke();
    }
}
