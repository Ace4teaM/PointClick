using UnityEngine;
using UnityEngine.InputSystem;

public class MovingController : MonoBehaviour
{
    private Camera mainCam;

    [SerializeField] private PolygonCollider2D walkingArea;
    public MoverAnimator moverAnimator;

    public PolygonCollider2D WalkingArea => walkingArea;

    // Cette fonction sera bindée dans Input Action
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed) // assure que c’est un clic, pas un relâché
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f; // pour 2D

            // recherche la collision la plus proche
            if (walkingArea.GetFirstIntersection(moverAnimator.transform.position, worldPos, out Vector2 hit))
            {
                Debug.DrawLine(moverAnimator.transform.position, hit, Color.red); // visualisation
                worldPos = new Vector3(hit.x, hit.y,0);
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
}
