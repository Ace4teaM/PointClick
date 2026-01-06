using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Controlleur permettant de déterminer l'objet en interaction avec le curseur
/// </summary>
public class InteractingController : MonoBehaviour
{
    public Collider2D[] interactingArea;
    Vector2 mouseWorld = Vector2.zero;
    Vector2 mousePos = Vector2.zero;
    bool onMove = false;

    // Cette fonction sera bindée dans Input Action
    internal void OnMove()
    {
        mousePos = Mouse.current.position.ReadValue();
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
        HoverCursorFlagStates.UnApply();
    }

    void Update()
    {
        if (onMove == true)
        {
            onMove = false;


            ///
            /// Détecte d'abord un élément de l'UI (screen space)
            ///
            if (EventSystem.current != null)
            {
                int uiLayerMask = LayerMask.GetMask("UI");
                int uiLayerIndex = LayerMask.NameToLayer("UI");

                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Mouse.current.position.ReadValue()
                };

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var r in results)
                {
                    if (r.gameObject.layer == uiLayerIndex)
                    {
                        var hover = r.gameObject.GetComponentInParent<IHoverCursorFlag>();
                        if (hover != null)
                        {
                            hover.Apply();
                            return;
                        }
                    }
                }
            }

            ///
            /// Détecte un objet du jeu (world space)
            ///
            var sortedHits = interactingArea.Where(p =>
                p.OverlapPoint(mouseWorld))
                .OrderBy(x =>
                    x.transform.position.y
                ).ToArray();

            if (sortedHits.Length == 0)
            {
                HoverCursorFlagStates.UnApply();
            }
            else
            {
                var hc = sortedHits[0].gameObject.GetComponent<IHoverCursorFlag>();
                if (hc != null)
                    hc.Apply();
                else
                    HoverCursorFlagStates.UnApply();
            }
        }
    }
}
