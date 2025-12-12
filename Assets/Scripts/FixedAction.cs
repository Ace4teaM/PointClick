using UnityEngine;

public class FixedAction : MonoBehaviour
{
    public ActionType actionType = ActionType.None;

    // Update is called once per frame
    void Update()
    {
        if (GameData.action != actionType)
        {
            GameData.action = actionType;
            GameData.OnActionChange();
        }
    }
}
