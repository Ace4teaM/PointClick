using UnityEngine;

public class InitStates : MonoBehaviour
{
    public enum SceneType
    {
        GameScene,
        UIScene
    }
    public SceneType sceneType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Stoke le nom de l'objet
        switch(sceneType)
        {
            case SceneType.GameScene:
                GameData.CurrentSceneGame = gameObject.scene.name;
                break;
            case SceneType.UIScene:
                GameData.CurrentSceneUI = gameObject.scene.name;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
