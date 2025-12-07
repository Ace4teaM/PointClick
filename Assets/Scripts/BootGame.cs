using UnityEngine;
using UnityEngine.SceneManagement;

public class BootGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameData.CurrentSceneGame = "Piece Principale";
        GameData.CurrentSceneUI = "GameUI";

        // Charger la scène persistante
        if (!SceneManager.GetSceneByName(GameData.CurrentSceneUI).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneUI, LoadSceneMode.Additive);
        if (!SceneManager.GetSceneByName(GameData.CurrentSceneGame).isLoaded)
            SceneManager.LoadScene(GameData.CurrentSceneGame, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
