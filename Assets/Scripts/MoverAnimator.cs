using UnityEngine;

/// <summary>
/// Déplace un point à l'écran en fonction de la destination donné
/// </summary>
public class MoverAnimator : MonoBehaviour
{
    public enum Direction : int
    {
       N,S,E,W,NE,SE,NW,SW
    }

    /// <summary>
    /// Obtient la direction la plus proche
    /// </summary>
    /// <param name="targetDirection"></param>
    /// <returns></returns>
    public static Direction GetClosestDirection(Vector3 targetDirection)
    {
        targetDirection.Normalize(); // normalise le vecteur cible

        int closest = 0;
        float maxDot = Vector3.Dot(targetDirection, gizmoDirections[0]);

        for (int i = 1; i < gizmoDirections.Length; i++)
        {
            float dot = Vector3.Dot(targetDirection, gizmoDirections[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = i;
            }
        }

        return (Direction)closest;
    }

    [SerializeField] public Direction direction;

    #region Gizmo
    [SerializeField] public float gizmoSize = 1.0f;
    private static  Vector3[] gizmoDirections = { 
        new Vector3(0,1,0).normalized,//N
        new Vector3(0,-1,0).normalized,//S
        new Vector3(1,0,0).normalized,//E
        new Vector3(-1,0,0).normalized,//W
        new Vector3(1,1,0).normalized,//NE
        new Vector3(1,-1,0).normalized,//SE
        new Vector3(-1,1,0).normalized,//NW
        new Vector3(-1,-1,0).normalized,//SW
    };

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        Gizmos.color = Color.orange;
        DrawArrow(transform.position, gizmoDirections[(int)direction], gizmoSize * 2);
    }

    private void DrawArrow(Vector3 start, Vector3 dir, float length)
    {
        float headAngle = 20f;             // angle de la pointe
        float headLength = 0.10f;          // longueur de la pointe

        Vector3 end = start + dir * length;

        // Ligne principale
        Gizmos.DrawLine(start, end);

        // Pointe de flèche
        Vector3 right = Quaternion.Euler(0, 0, headAngle) * -dir;
        Vector3 left = Quaternion.Euler(0, 0, -headAngle) * -dir;
        Gizmos.DrawLine(end, end + right * headLength);
        Gizmos.DrawLine(end, end + left * headLength);
    }
    #endregion

    [SerializeField] private Vector3 destination;

    public Vector3 Destination => destination;


    public float speed = 5f;

    private bool hasDestination = false;

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
        hasDestination = true;
    }
    void Update()
    {
        if (!hasDestination)
            return;

        // incrémente la position actuelle vers la destination
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        // obtient la direction la plus proche du vecteur de destination
        direction = GetClosestDirection((destination - transform.position).normalized);

        // Arrivé au point
        if (Vector3.Distance(transform.position, destination) < 0.05f)
            hasDestination = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
