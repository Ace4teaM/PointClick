using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InspectingController : MonoBehaviour
{
    /// <summary>
    /// Si true l'utiliseur a cliquer pour déplacer l'objet
    /// Cette propriété est utilisé en décalage avec OnClick et Update pour permettre à Unity de calculer toutes les propriétés d'UI avant l'action (ie: EventSystem.current.IsPointerOverGameObject())
    /// </summary>
    private bool wantInspect = false;
    void Awake()
    {
        GameData.InputClickEvent += OnClick;
    }
    void OnDestroy()
    {
        GameData.InputClickEvent -= OnClick;
    }

    // Cette fonction sera bindée dans Input Action
    internal void OnClick()
    {
        // Vérifie si l'action en cours est "Inspecter"
        if (GameData.action != ActionType.Inspect)
            return;

        wantInspect = true;
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
                case "Porte":
                    SceneTransition.SetTransition(Scenes.BoitesAuSol);
                    break;
                case "Bibliothèque":
                    SceneTransition.SetTransition(Scenes.Bibliotheque);
                    break;
                case "Canapé":
                    break;
                case "Seb":
                    break;
            }
        }
    }
}
