using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Gestion du curseur à l'écran
/// </summary>
public class CursorManager : MonoBehaviour
{
    public Texture2D uiCursor;
    public Texture2D moveCursor;
    public Texture2D inspectCursor;
    public Texture2D talkCursor;
    public Texture2D actionCursor;

    public Vector2 uiHotspot;
    public Vector2 moveHotspot;
    public Vector2 inspectHotspot;
    public Vector2 talkHotspot;
    public Vector2 actionHotspot;

    public void SetMove() => Cursor.SetCursor(moveCursor, moveHotspot, CursorMode.Auto);
    public void SetInspect() => Cursor.SetCursor(inspectCursor, inspectHotspot, CursorMode.Auto);
    public void SetTalk() => Cursor.SetCursor(talkCursor, talkHotspot, CursorMode.Auto);
    public void SetAction() => Cursor.SetCursor(actionCursor, actionHotspot, CursorMode.Auto);

    void Awake()
    {
    }

    private void OnEnable()
    {
        // S'abonner à l'event global
        GameData.OnActionChanged += OnActionChanged;
    }

    private void OnDisable()
    {
        // Se désabonner pour éviter les fuites
        GameData.OnActionChanged -= OnActionChanged;
    }

    // Fonction appelée quand l'état global change
    private void OnActionChanged(ActionType value)
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        // Le curseur se trouve sur un élément de l’UI (Button ou autre)
        if (HoverCursorFlag.HoverFlagType == HoverFlagType.UI)
        {
            Cursor.SetCursor(uiCursor, uiHotspot, CursorMode.Auto);
        }
        else
        {
            switch (GameData.action)
            {
                case ActionType.Move:
                    Cursor.SetCursor(moveCursor, moveHotspot, CursorMode.Auto);
                    break;
                case ActionType.Inspect:
                    Cursor.SetCursor(inspectCursor, inspectHotspot, CursorMode.Auto);
                    break;
                case ActionType.Talk:
                    Cursor.SetCursor(talkCursor, talkHotspot, CursorMode.Auto);
                    break;
                case ActionType.Activate:
                    Cursor.SetCursor(actionCursor, actionHotspot, CursorMode.Auto);
                    break;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCursor();
    }

    HoverFlagType lastHoverType = HoverCursorFlag.HoverFlagType;
    
    void Update()
    {
        // Le curseur se trouve sur un élément de l’UI (Button ou autre)
        if (lastHoverType != HoverCursorFlag.HoverFlagType)
        {
            UpdateCursor();
            lastHoverType = HoverCursorFlag.HoverFlagType;
        }
    }
}
