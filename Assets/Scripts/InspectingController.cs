using UnityEngine;
using UnityEngine.InputSystem;

public class InspectingController : MonoBehaviour
{
    /// <summary>
    /// Si true l'utiliseur a cliquer pour déplacer l'objet
    /// Cette propriété est utilisé en décalage avec OnClick et Update pour permettre à Unity de calculer toutes les propriétés d'UI avant l'action (ie: EventSystem.current.IsPointerOverGameObject())
    /// </summary>
    private bool wantInspect = false;

    // Cette fonction sera bindée dans Input Action
    public void OnClick(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (!context.control.IsPressed())
            return;

        // Vérifie si l'action en cours est "Déplacer"
        if (GameData.action != ActionType.Inspect)
            return;


        if (context.performed) // assure que c’est un clic, pas un relâché
        {
            wantInspect = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (wantInspect)
        {
            wantInspect = false;
            // Le clic vient de l’UI (Button ou autre)
            if (HoverCursorFlag.HoverFlagType == HoverFlagType.UI)
                return;

            switch (HoverCursorFlag.HoverFlag)
            {
                case "Bibliothèque":
                    break;
                case "Canapé":
                    break;
                case "Seb":
                    break;
            }
        }
    }
}
