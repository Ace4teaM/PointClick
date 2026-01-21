using UnityEngine;

[CreateAssetMenu(menuName = "Game/RestoreStateUserInterface")]
public class RestoreStateUserInterface : RestorableStates
{
    public override string ID => "UserInterface";

    public override string CaptureState(GameObject obj)
    {
        return GameData.CurrentSceneUI;
    }

    public override void RestoreState(GameObject obj, string state)
    {
        SceneTransition.ChangeUI(state);
    }
}
