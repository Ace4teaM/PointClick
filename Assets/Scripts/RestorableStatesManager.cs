using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage les états des objets
/// </summary>
public class RestorableStatesManager : MonoBehaviour
{
    Dictionary<string, object> sceneState = new();

    public void Save()
    {
        foreach (var s in FindObjectsByType<InitStates>(FindObjectsSortMode.None))
        {
        }
    }

    public void Load()
    {
        foreach (var s in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (s is IRestorableStates saveable &&
                sceneState.TryGetValue(saveable.ID, out var state))
            {
            }
        }
    }
}