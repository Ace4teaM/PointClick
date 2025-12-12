using UnityEngine;

public class FixedCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 cursorHotspot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}
