using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Collections.Generic;

/// <summary>
/// MovingController à pour fonction de réagir aux événements de la souris
/// pour determiner le chemin directe entre la position actuelle et la destination
/// puis de passer la droite au MoverAnimator pour animation
/// </summary>
public class MovingController : MonoBehaviour
{
    private Camera mainCam;

    [SerializeField] private PolygonCollider2D[] walkingArea;
    [SerializeField] private PathFinder walkingPath;
    public MoverAnimator moverAnimator;

    public PolygonCollider2D[] WalkingArea => walkingArea;
    public PathFinder WalkingPath => walkingPath;

    // Offset entre le segment de collision et la position réel de déplacement
    // Evite les problèmes de traverser des murs lié à la précision des floats
    public float moveOffset = 0.1f;

    // Obtient le point de destination d'un vecteur avec un offset
    // Cette méthode est utilisé pour ne pas déplacer le personnage exactement à l'emplacement de la collision mais juste avant
    public Vector3 GetPointBeforeDestination(Vector3 start, Vector3 destination, float offset)
    {
        Vector3 dir = destination - start;
        float dist = dir.magnitude;

        if (dist <= offset)
            return start;

        return destination - dir.normalized * offset;
    }

    internal bool AdjustDestinationPoint(Vector3 startPos, ref Vector3 worldPos)
    {
        // recherche la collision la plus proche
        Vector2? hit = null;
        foreach (var area in walkingArea)
        {
            if (area.GetFirstIntersection(startPos, worldPos, out var found))
            {
                if (hit == null || (hit != null && (Vector2.Distance(startPos, found) < Vector2.Distance(startPos, hit.Value))))
                {
                    hit = GetPointBeforeDestination(startPos, found, moveOffset);
                }
            }
        }

        if (hit != null)
        {
            worldPos = new Vector3(hit.Value.x, hit.Value.y, 0);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Si true l'utiliseur a cliquer pour déplacer l'objet
    /// Cette propriété est utilisé en décalage avec OnClick et Update pour permettre à Unity de calculer toutes les propriétés d'UI avant l'action (ie: EventSystem.current.IsPointerOverGameObject())
    /// </summary>
    private bool wantMove = false;

    // Cette fonction sera bindée dans Input Action
    public void OnClick(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (!context.control.IsPressed())
            return;

        // Vérifie si l'action en cours est "Déplacer"
        if (GameData.action != ActionType.Move)
            return;


        if (context.performed) // assure que c’est un clic, pas un relâché
        {
            wantMove = true;
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
        if(wantMove)
        {
            wantMove = false;

            // Le clic vient de l’UI (Button ou autre)
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 targetPos = mainCam.ScreenToWorldPoint(mousePos);

            Vector3 startPos = moverAnimator.walkingPoint.position;

            // Calcule le chemin le plus directe
            // Si la destination n'est pas direct et qu'un chemin est disponible
            // pour guider le déplacement, on l'utilise
            Vector3 worldPos = targetPos;
            worldPos.z = 0f; // pour 2D
            if (AdjustDestinationPoint(startPos, ref worldPos) == true && walkingPath != null)
            {
                worldPos = targetPos;
                worldPos.z = 0f;

                var path = walkingPath.FindPath(moverAnimator.walkingPoint.position, worldPos);

                // recherche le chemin de départ le plus direct en partant du dernier noeud
                for(int i = path.Count-1; i>=0;i--)
                {
                    bool found = false;
                    foreach (var area in walkingArea)
                    {
                        if ((found = area.GetFirstIntersection(startPos, path.ElementAt(i), out var hit)))
                        {
                            break;
                        }
                    }

                    // si il n'y a aucune collision, alors on peut se rendre directement à ce point
                    if (!found)
                    {
                        for(int j=0;j<i;j++)
                            path.Dequeue();
                        break;
                    }
                }

                // recherche le chemin d'arrivé le plus direct en partant du premier noeud
                for (int i = 0; i < path.Count; i++)
                {
                    bool found = false;
                    foreach (var area in walkingArea)
                    {
                        if ((found = area.GetFirstIntersection(worldPos, path.ElementAt(i), out var hit)))
                        {
                            break;
                        }
                    }

                    // si il n'y a aucune collision, alors on peut se rendre directement à ce point
                    if (!found)
                    {
                        Queue<Vector3> queue = new Queue<Vector3>();

                        // Remplissage
                        for (int j = 0; j < i+1; j++)
                            queue.Enqueue(path.Dequeue());

                        path = queue;

                        break;
                    }
                }

                // ajoute le dernier segment du déplacement
                if (path.Count > 0)
                {
                    AdjustDestinationPoint(path.Last(), ref worldPos);
                    path.Enqueue(worldPos);
                }

                moverAnimator.SetDestinations(path);
            }
            else
            {
                moverAnimator.SetDestination(worldPos);
            }

            lastCollisionPointFrom = startPos;
            lastCollisionPointTo = worldPos;
        }
    }

    #region Gizmo
    private Vector3 lastCollisionPointFrom;
    private Vector3 lastCollisionPointTo;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(lastCollisionPointFrom, lastCollisionPointTo);
    }
    #endregion
}
