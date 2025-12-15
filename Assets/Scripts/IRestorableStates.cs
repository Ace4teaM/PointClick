using UnityEngine;
/// <summary>
/// Interface pour les objets ayant des états sauvegardeable/restorable
/// </summary>
public interface IRestorableStates
{
    string ID { get; }
    string CaptureState(GameObject obj);
    void RestoreState(GameObject obj, string state);
}

public abstract class RestorableStates : ScriptableObject, IRestorableStates
{
    public abstract string ID { get; }
    public abstract string CaptureState(GameObject obj);
    public abstract void RestoreState(GameObject obj, string state);
}