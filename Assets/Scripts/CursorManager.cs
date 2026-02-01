using Unity.VisualScripting;
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
    public Texture2D interactCursor;

    public Vector2 uiHotspot;
    public Vector2 moveHotspot;
    public Vector2 inspectHotspot;
    public Vector2 talkHotspot;
    public Vector2 actionHotspot;
    public Vector2 interactHotspot;

    public void SetMove() => Cursor.SetCursor(moveCursor, new Vector2(moveCursor.width, moveCursor.height) * moveHotspot, CursorMode.Auto);
    public void SetInspect() => Cursor.SetCursor(inspectCursor, new Vector2(inspectCursor.width, inspectCursor.height) * inspectHotspot, CursorMode.Auto);
    public void SetTalk() => Cursor.SetCursor(talkCursor, new Vector2(talkCursor.width, talkCursor.height) * talkHotspot, CursorMode.Auto);
    public void SetAction() => Cursor.SetCursor(actionCursor, new Vector2(actionCursor.width, actionCursor.height) * actionHotspot, CursorMode.Auto);
    public void SetInteract() => Cursor.SetCursor(interactCursor, new Vector2(interactCursor.width, interactCursor.height) * interactHotspot, CursorMode.Auto);
    public void SetUI() => Cursor.SetCursor(uiCursor, new Vector2(uiCursor.width, uiCursor.height) * uiHotspot, CursorMode.Auto);

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
        if (HoverCursorFlagStates.HoverFlagType == HoverFlagType.UI || HoverCursorFlagStates.HoverFlagType == HoverFlagType.GameItem)
        {
            SetUI();
        }
        else
        {
            switch (GameData.action)
            {
                case ActionType.Move:
                    SetMove();
                    break;
                case ActionType.Inspect:
                    SetInspect();
                    break;
                case ActionType.Talk:
                    SetTalk();
                    break;
                case ActionType.Activate:
                    SetAction();
                    break;
                case ActionType.Interact:
                    SetInteract();
                    break;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCursor();
    }

    HoverFlagType lastHoverType = HoverCursorFlagStates.HoverFlagType;
    
    void Update()
    {
        // Le curseur se trouve sur un élément de l’UI (Button ou autre)
        if (lastHoverType != HoverCursorFlagStates.HoverFlagType)
        {
            UpdateCursor();
            lastHoverType = HoverCursorFlagStates.HoverFlagType;
        }
    }
}
