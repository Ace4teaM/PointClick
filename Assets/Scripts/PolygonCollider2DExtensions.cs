using UnityEngine;

public static class PolygonCollider2DExtensions
{
    /// <summary>
    /// Calculer le premier point d’intersection avec un PolygonCollider2D
    /// </summary>
    public static bool GetFirstIntersection(this PolygonCollider2D polygon, Vector2 start, Vector2 end, out Vector2 hitPoint)
    {
        hitPoint = Vector2.zero;
        bool found = false;
        float closestDist = float.MaxValue;

        for (int p = 0; p < polygon.pathCount; p++)
        {
            Vector2[] points = polygon.GetPath(p);
            int len = points.Length;

            for (int i = 0; i < len; i++)
            {
                Vector2 a = polygon.transform.TransformPoint(points[i]);
                Vector2 b = polygon.transform.TransformPoint(points[(i + 1) % len]);

                if (UtilsGeometry2D.SegmentIntersect(start, end, a, b, out Vector2 intersection))
                {
                    float dist = Vector2.SqrMagnitude(intersection - start);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        hitPoint = intersection;
                        found = true;
                    }
                }
            }
        }

        return found;
    }
}
