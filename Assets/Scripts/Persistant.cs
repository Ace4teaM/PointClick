using UnityEngine;

/// <summary>
/// Singleton contenant les données de jeu persistants entre les scènes
/// </summary>
[DefaultExecutionOrder(-1000)] // s'execute en premier (Valeur négative = plus tôt, Valeur positive = plus tard)
public class Persistant : MonoBehaviour
{
    public static Persistant Instance;

    // noms des items de l'inventaire
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
