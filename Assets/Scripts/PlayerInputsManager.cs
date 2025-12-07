using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Rediriger les appels aux Inputs Player vers les événements délégué aux scnèes actives
/// </summary>
public class PlayerInputsManager : MonoBehaviour
{
    // Cette fonction sera bindée dans Input Action
    public void OnClick(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (!context.control.IsPressed())
            return;

        if (context.performed) // assure que c’est un clic, pas un relâché
        {
            GameData.OnInputClick();
        }
    }

}
