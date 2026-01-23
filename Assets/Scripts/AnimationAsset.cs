using System;
using UnityEngine;

public class AnimationAsset : MonoBehaviour
{
    public void OnAnimate(Animations anim, string animationName)
    {
        switch (animationName)
        {
            case "Fred se lève du canapé":
                {
                    anim.ChangeState("Fred", "IsSat", false);
                    anim.MoveTo("Fred", "Canapé");
                    anim.start = true;
                }
                break;
            case "Les boites tombent sur Fred":
                {
                    anim.Wait(1000);
                    anim.ChangeState("Fred", "IsDizzy", true);
                    anim.Wait(1000);
                    anim.ShowDialog("Seb: Ca va Fred ?");
                    anim.ShowDialog("Seb: Tu te sent bien ?");
                    anim.HideDialog();
                    anim.Wait(2000);
                    anim.start = true;
                }
                break;
            case "Animation du tonnerre":
                {
                }
                break;
            case "Afficher les éléments achetables":
                {
                    GameData.ShowDialogChoices = new string[]{
                        "Magazine Elle&Lui",
                        "Paquet de bonbons",
                        "Médicament",
                        "Retour"
                    };
                }
                break;
            case "L'Agent part immédiatement aux toilettes, on entend des bruits à travers la porte":
                {
                    GameObject.Find("Agent")?.SetActive(false);
                    GameObject.Find("Agent_1")?.SetActive(false);
                }
                break;
            case "Faire disparaitre l'objet brillant":
                {
                    GameObject.Find("Pièces de monnaies")?.SetActive(false);
                }
                break;
            case "L'Agent retourne au guichet":
                {
                    Animations.FindInactiveInScenes("Agent")?.SetActive(true);
                    Animations.FindInactiveInScenes("Agent_1")?.SetActive(true);
                }
                break;
            default:
                throw new Exception($"Impossible de déterminer l'animation nommée: '{animationName}'");
        }
    }
}
