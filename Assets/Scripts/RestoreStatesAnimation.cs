using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/RestoreStatesAnimation")]
public class RestoreStatesAnimation : RestorableStates
{
    public override string ID => "Animation." + filedName;
    public string filedName;

    public override string CaptureState(GameObject obj)
    {
        var animator = obj.GetComponentInChildren<Animator>();
        var param = animator.parameters.First(p => p.name == filedName);
        //return param.defaultBool.ToString();
        switch (param.type)
        {
            case AnimatorControllerParameterType.Int:
                return animator.GetInteger(filedName).ToString();
            case AnimatorControllerParameterType.Float:
                return animator.GetFloat(filedName).ToString();
            case AnimatorControllerParameterType.Bool:
                return animator.GetBool(filedName).ToString();
        }
        return string.Empty;
    }

    public override void RestoreState(GameObject obj, string state)
    {
        var animator = obj.GetComponentInChildren<Animator>();
        var param = animator.parameters.First(p => p.name == filedName);
        //param.defaultBool = bool.Parse(state);
        switch(param.type)
        {
            case AnimatorControllerParameterType.Int:
                animator.SetInteger(filedName, int.Parse(state));
                break;
            case AnimatorControllerParameterType.Float:
                animator.SetFloat(filedName, float.Parse(state));
                break;
            case AnimatorControllerParameterType.Trigger:
                animator.SetTrigger(filedName);
                break;
            case AnimatorControllerParameterType.Bool:
                animator.SetBool(filedName, bool.Parse(state));
                break;
        }
    }
}

