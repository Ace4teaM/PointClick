using UnityEngine;

[CreateAssetMenu(menuName = "Game/RestoreStatesPosition")]
public class RestoreStatesPosition : RestorableStates
{
    public override string ID => "Position";

    public override string CaptureState(GameObject obj)
    {
        var transform = obj.GetComponent<Transform>();
        return transform.position.ToParseString();
    }

    public override void RestoreState(GameObject obj, string state)
    {
        var transform = obj.GetComponent<Transform>();
        transform.position = Vector3Extensions.Parse(state);
    }
}
