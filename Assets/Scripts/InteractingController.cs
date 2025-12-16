using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlleur réalisant permettant de déterminer l'objet en interaction avec le curseur
/// </summary>
public class InteractingController : MonoBehaviour
{
    public Collider2D[] interactingArea;
    Vector2 mouseWorld = Vector2.zero;
    bool onMove = false;

    // Cette fonction sera bindée dans Input Action
    internal void OnMove()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);

        onMove = true;
    }

    void Awake()
    {
        GameData.InputMoveEvent += OnMove;
    }
    void OnDestroy()
    {
        GameData.InputMoveEvent -= OnMove;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HoverCursorFlag.UnApply();
    }

    void Update()
    {
        if (onMove == true)
        {
            onMove = false;

            var sortedHits = interactingArea.Where(p => 
                p.OverlapPoint(mouseWorld))
                .OrderBy(x =>
                    x.transform.position.y
                ).ToArray();

            if (sortedHits.Length == 0)
            {
                HoverCursorFlag.UnApply();
            }
            else
            {
                var hc = sortedHits[0].gameObject.GetComponent<HoverCursorFlag>();
                if(hc != null)
                    hc.Apply();
                else
                    HoverCursorFlag.UnApply();
            }
        }
    }
}
