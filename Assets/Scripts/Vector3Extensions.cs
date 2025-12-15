using System.Globalization;
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

    /// <summary>
    /// Parse un Vector3 depuis une string au format "x,y,z".
    /// </summary>
    /// <param name="s">La chaîne à parser.</param>
    /// <returns>Le Vector3 correspondant.</returns>
    public static Vector3 Parse(this string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new System.ArgumentNullException(nameof(s), "La chaîne ne peut pas être vide.");

        // Sépare la chaîne sur la virgule
        string[] parts = s.Split(',');

        if (parts.Length != 3)
            throw new System.FormatException("La chaîne doit contenir exactement 3 valeurs séparées par des virgules.");

        // Utilisation de InvariantCulture pour éviter les problèmes de format (1.0 vs 1,0)
        float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
        float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
        float z = float.Parse(parts[2], CultureInfo.InvariantCulture);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Convertit un Vector3 en string au format "x,y,z" compatible avec Parse.
    /// </summary>
    public static string ToParseString(this Vector3 v)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1},{2}",
            v.x, v.y, v.z
        );
    }
}