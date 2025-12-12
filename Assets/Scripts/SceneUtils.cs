using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    /// <summary>
    /// Recherche un GameObject par son nom dans une scène,
    /// même s'il est désactivé, en parcourant toute la hiérarchie.
    /// </summary>
    public static GameObject GetObjectByName(string sceneName, string objectName)
    {
        // Récupère la scène
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid())
        {
            Debug.LogError($"Scene '{sceneName}' introuvable ou non chargée.");
            return null;
        }

        // Récupère les objets racine de la scène
        GameObject[] roots = scene.GetRootGameObjects();

        foreach (var root in roots)
        {
            // 🔍 Cherche dans l'objet lui-même
            if (root.name == objectName)
                return root;

            // 🔍 Cherche dans ses enfants (même désactivés)
            GameObject result = FindInChildren(root.transform, objectName);
            if (result != null)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Recherche récursive dans tous les enfants.
    /// Fonctionne sur objets actifs et inactifs.
    /// </summary>
    private static GameObject FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject result = FindInChildren(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
