using UnityEngine;

/// <summary>
/// Singleton contenant les données de jeu persistants entre les scènes
/// </summary>
public class Persistant : MonoBehaviour
{
    public static Persistant Instance;

    // Exemple de variables globales
    public string[] inventoryItems = new string[4];

    private void Awake()
    {
        // Si une instance existe déjà, détruire le doublon
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Sinon définir l’unique instance
        Instance = this;

        // Rendre persistant
        DontDestroyOnLoad(gameObject);
    }
}
