using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Retourne le vecteur normalisé pointant de "from" vers "to".
    /// </summary>
    public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    /// <summary>
    /// Retourne l'angle en degrés autour de Z pointant de "from" vers "to" (utile pour 2D).
    /// </summary>
    public static float AngleTo(this Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from).normalized;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}