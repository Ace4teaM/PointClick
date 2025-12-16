using UnityEngine;

[CreateAssetMenu(menuName = "Game/RestoreStatesActive")]
public class RestoreStatesActive : RestorableStates
{
    public override string ID => "Enable";

    public override string CaptureState(GameObject obj)
    {
        return obj.activeSelf.ToString();
    }

    public override void RestoreState(GameObject obj, string state)
    {
        obj.SetActive(bool.Parse(state));
    }
}
