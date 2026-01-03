using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootGame : MonoBehaviour
{
    /// <summary>
    /// Liste de tous les items du jeu
    /// </summary>
    /// <remarks>Asset des items (pas les items présent dans l'inventaire)</remarks>
    [SerializeField]
    public InventoryItem[] GameItems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // initialise l'inventaire
        // copie les items de la sauvegarde persistante vers les données du jeu en cours
        int i = 0;
        foreach (var item in Persistant.Instance.inventoryItems)
        {
            GameData.InventoryItems[i] = GameItems.First(p => p.label == item);
            i++;
        }

        // Copie les items dans la variable statique pour utilisation en jeu
        GameData.GameItems = GameItems;

        // Conserve une référence sur l'item vide
        InventoryItem.Empty = GameItems.First(p=>String.IsNullOrEmpty(p.label));

        // Charger la scène persistante
        if (!string.IsNullOrEmpty(GameData.CurrentSceneUI) && !SceneManager.GetSceneByName(GameData.CurrentSceneUI).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneUI, LoadSceneMode.Additive);
        if (!string.IsNullOrEmpty(GameData.CurrentSceneGame) && !SceneManager.GetSceneByName(GameData.CurrentSceneGame).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneGame, LoadSceneMode.Additive);

        // Evenements d'initialisation
        GameData.OnInventoryChange();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
