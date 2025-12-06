using UnityEngine;

/// <summary>
/// Instance locale permettant d'interagir avec les données globales (static)
/// Peut être instancié dans chaque scène de jeu pour réaliser des bindings
/// </summary>
public class GameDataHandler : MonoBehaviour
{
    public void EnableMove()
    {
        GameData.action = ActionType.Move;
        GameData.OnActionChange();
    }

    public void EnableInspect()
    {
        GameData.action = ActionType.Inspect;
        GameData.OnActionChange();
    }

    public void EnableTalk()
    {
        GameData.action = ActionType.Talk;
        GameData.OnActionChange();
    }

    public void EnableActivate()
    {
        GameData.action = ActionType.Activate;
        GameData.OnActionChange();
    }
}
