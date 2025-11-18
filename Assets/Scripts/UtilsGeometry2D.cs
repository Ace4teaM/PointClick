using UnityEngine;

public static class UtilsGeometry2D
{
    /// <summary>
    /// Retourne vrai si les segments (p, p2) et (q, q2) s'intersectent et retourne le point d'intersection
    /// </summary>
    public static bool SegmentIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 r = p2 - p;
        Vector2 s = q2 - q;
        float rxs = r.x * s.y - r.y * s.x;
        float qpxr = (q - p).x * r.y - (q - p).y * r.x;

        if (Mathf.Approximately(rxs, 0f) && Mathf.Approximately(qpxr, 0f))
        {
            // Colinéaire, on peut gérer si nécessaire
            return false;
        }

        if (Mathf.Approximately(rxs, 0f) && !Mathf.Approximately(qpxr, 0f))
            return false; // parallèles et non colinéaires

        float t = ((q - p).x * s.y - (q - p).y * s.x) / rxs;
        float u = ((q - p).x * r.y - (q - p).y * r.x) / rxs;

        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            intersection = p + t * r;
            return true;
        }

        return false;
    }
}
