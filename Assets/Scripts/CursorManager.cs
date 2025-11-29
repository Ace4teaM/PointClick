using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D moveCursor;
    public Texture2D inspectCursor;
    public Texture2D talkCursor;
    public Texture2D actionCursor;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCursor();
    }
}
