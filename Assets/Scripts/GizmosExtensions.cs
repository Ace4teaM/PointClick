using UnityEngine;

public static class GizmosExtensions
{
    public static void DrawArrow(Vector3 start, Vector3 dir, float length)
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

    public static void DrawArrow(Vector3 start, Vector3 end)
    {
        float headAngle = 20f;             // angle de la pointe
        float headLength = 0.10f;          // longueur de la pointe

        Vector2 dir = (end - start).normalized;

        // Ligne principale
        Gizmos.DrawLine(start, end);

        // Pointe de flèche
        Vector3 right = Quaternion.Euler(0, 0, headAngle) * -dir;
        Vector3 left = Quaternion.Euler(0, 0, -headAngle) * -dir;
        Gizmos.DrawLine(end, end + right * headLength);
        Gizmos.DrawLine(end, end + left * headLength);
    }

}
