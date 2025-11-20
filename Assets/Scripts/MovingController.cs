using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingController : MonoBehaviour
{
    private Camera mainCam;

    [SerializeField] private PolygonCollider2D[] walkingArea;
    public MoverAnimator moverAnimator;

    public PolygonCollider2D[] WalkingArea => walkingArea;

    // Cette fonction sera bindée dans Input Action
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed) // assure que c’est un clic, pas un relâché
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f; // pour 2D

            // recherche la collision la plus proche
            Vector2? hit = null;
            foreach (var area in walkingArea)
            {
                if (area.GetFirstIntersection(moverAnimator.walkingPoint.position, worldPos, out var found))
                {
                    if(hit == null || (hit != null && (Vector2.Distance(moverAnimator.walkingPoint.position, found) < Vector2.Distance(moverAnimator.walkingPoint.position, hit.Value))))
                    {
                        hit = found;
                    }
                }
            }

            if (hit != null)
            {
                lastCollisionPointFrom = moverAnimator.walkingPoint.position;
                lastCollisionPointTo = hit.Value;
                worldPos = new Vector3(hit.Value.x, hit.Value.y,0);
            }
            else
            {
                lastCollisionPointFrom = moverAnimator.walkingPoint.position;
                lastCollisionPointTo = worldPos;
            }

            moverAnimator.SetDestination(worldPos);
        }
    }

    void Awake()
    {
        mainCam = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Gizmo
    [SerializeField] public Vector3 lastCollisionPointFrom;
    [SerializeField] public Vector3 lastCollisionPointTo;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(lastCollisionPointFrom, lastCollisionPointTo);
    }
    #endregion
}
