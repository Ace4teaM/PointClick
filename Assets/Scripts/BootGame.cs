using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


[DefaultExecutionOrder(-100)] // s'execute en premier (Valeur négative = plus tôt, Valeur positive = plus tard)
public class BootGame : MonoBehaviour
{
    /// <summary>
    /// Liste de tous les items du jeu
    /// </summary>
    /// <remarks>Asset des items (pas les items présent dans l'inventaire)</remarks>
    [SerializeField]
    public InventoryItem[] GameItems;

    void Awake()
    {
        // Conserve une référence sur l'item vide
        InventoryItem.Empty = GameItems.First(p => String.IsNullOrEmpty(p.label));

        // Aucune sélection par défaut
        GameData.SelectedInventoryItem = InventoryItem.Empty;

        // initialise l'inventaire
        // copie les items de la sauvegarde persistante vers les données du jeu en cours
        int i = 0;
        GameData.InventoryItems = new InventoryItem[Persistant.Instance.inventoryItems.Length];
        foreach (var item in Persistant.Instance.inventoryItems)
        {
            GameData.InventoryItems[i] = GameItems.First(p => p.label == item);
            i++;
        }

        // Copie les items dans la variable statique pour utilisation en jeu
        GameData.GameItems = GameItems;

        // Référence l'instance du GameGraph
        GameData.GameGraph = SceneUtils.GetObjectByName("Main", "GameGraph").GetComponent<GameGraph>();

        // Charger la scène persistante (si des scènes sont déja chargé dans l'éditeur)
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(GameData.CurrentSceneUI) && !SceneManager.GetSceneByName(GameData.CurrentSceneUI).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneUI, LoadSceneMode.Additive);
        if (!string.IsNullOrEmpty(GameData.CurrentSceneGame) && !SceneManager.GetSceneByName(GameData.CurrentSceneGame).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneGame, LoadSceneMode.Additive);
#else
        GlobalGameGraph.Instance.SetStates(0, 'A', true);
        SceneManager.LoadScene("Accueil", LoadSceneMode.Additive);
#endif
    }

    void Start()
    {
        // Evenements d'initialisation
        GameData.OnInventoryChange();
    }

    void Update()
    {
        
    }
}
