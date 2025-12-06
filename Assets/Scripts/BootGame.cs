using UnityEngine;
using UnityEngine.SceneManagement;

public class BootGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Charger la scène persistante
        if (!SceneManager.GetSceneByName("GameUI").isLoaded)
            SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);
        if (!SceneManager.GetSceneByName("Piece Principale").isLoaded)
            SceneManager.LoadScene("Piece Principale", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
