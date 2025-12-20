using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootGame : MonoBehaviour
{
    /// <summary>
    /// Liste des items du jeu
    /// </summary>
    [SerializeField]
    public InventoryItem[] GameItems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if UNITY_EDITOR
        Persistant.Instance.inventoryItems[0] = "Billets d'avions";
        Persistant.Instance.inventoryItems[1] = "Pièces de monnaies";
        Persistant.Instance.inventoryItems[2] = "";
        Persistant.Instance.inventoryItems[3] = "";
#endif
        // initialise l'inventaire
        int i = 0;
        foreach (var item in Persistant.Instance.inventoryItems)
        {
            GameData.InventoryItems[i] = GameItems.First(p => p.label == item);
            i++;
        }

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
