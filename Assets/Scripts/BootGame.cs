using UnityEngine;
using UnityEngine.SceneManagement;

public class BootGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Charger la scène persistante
        if (!string.IsNullOrEmpty(GameData.CurrentSceneUI) && !SceneManager.GetSceneByName(GameData.CurrentSceneUI).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneUI, LoadSceneMode.Additive);
        if (!string.IsNullOrEmpty(GameData.CurrentSceneGame) && !SceneManager.GetSceneByName(GameData.CurrentSceneGame).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneGame, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
