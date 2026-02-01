using System;
using System.Linq;
using UnityEngine;

public class AnimationAsset_Grenier : AnimationAsset
{
    internal static readonly float baseLuminosityLumiere = 0.8f;
    internal static readonly float baseLuminosityTonnerre = 0.15f;
    public override void OnAnimate(Animations anim, string animationName)
    {
        switch (animationName)
        {
            case "Fred se lève du canapé":
                {
                    anim.ChangeState("Fred", "IsSat", false);
                    anim.MoveTo("Fred", "Canapé");
                    anim.Execute();
                }
                break;
            case "Fred est assit":
                {
                    anim.Disable("boite de jeu");
                    anim.ChangeState("Fred", "SatState", 0f);
                    anim.Execute();
                }
                break;
            case "Fred est assit Absurde":
                {
                    anim.Disable("boite de jeu");
                    anim.ChangeState("Fred", "SatState", 1f);
                    anim.Execute();
                }
                break;
            case "Fred est assit Dépité":
                {
                    anim.Disable("boite de jeu");
                    anim.ChangeState("Fred", "SatState", 2f);
                    anim.Execute();
                }
                break;
            case "Fred est assit Mains au ciel":
                {
                    anim.Disable("boite de jeu");
                    anim.ChangeState("Fred", "SatState", 3f);
                    anim.Execute();
                }
                break;
            case "Fred est assit Présente":
                {
                    anim.Enable("boite de jeu");
                    anim.ChangeState("Fred", "SatState", 4f);
                    anim.Execute();
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
                    anim.Execute();
                }
                break;
            case "Assombrissement":
                {
                    anim.Enable("Sound Pluie");
                    anim.ChangeProperty<LuminosityParameters>("Tonnerre", nameof(LuminosityParameters.GlobalAlpha), baseLuminosityTonnerre);
                    anim.ChangeProperty<LuminosityParameters>("Luminosité", nameof(LuminosityParameters.GlobalAlpha), 0f);
                    anim.UpTo<LuminosityParameters>("Luminosité", nameof(LuminosityParameters.GlobalAlpha), 0.3f, baseLuminosityLumiere);
                    anim.Execute();
                }
                break;
            case "Animation du tonnerre":
                {
                    anim.ChangeProperty<LuminosityParameters>("Tonnerre", nameof(LuminosityParameters.GlobalAlpha), UnityEngine.Random.Range(0.6f, 0.8f));
                    anim.UpTo<LuminosityParameters>("Tonnerre", nameof(LuminosityParameters.GlobalAlpha), -0.4f, baseLuminosityTonnerre);
                    if(UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    {
                        anim.ChangeProperty<LuminosityParameters>("Tonnerre", nameof(LuminosityParameters.GlobalAlpha), UnityEngine.Random.Range(0.6f, 0.8f));
                        anim.UpTo<LuminosityParameters>("Tonnerre", nameof(LuminosityParameters.GlobalAlpha), -0.4f, baseLuminosityTonnerre);
                    }
                    anim.Execute();
                }
                break;
            case "Musique de la petite voix":
                {
                    anim.Enable("Music Zelda");
                    anim.Execute();
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
                    GameData.OnChoicesChange();
                }
                break;
            case "L'Agent part immédiatement aux toilettes, on entend des bruits à travers la porte":
                {
                    anim.MoveTo("Agent_1", "Agent 1 Walking Path", GameObject.Find("WC").transform.position);
                    anim.Disable("Agent_1");
                    anim.Wait(500);
                    anim.ChangeProperty<MoverAnimator>("Fred", "direction", (int)MoverAnimator.Directions.N);
                    anim.Wait(500);
                    anim.ShowDialog("Fred: J'ai l'impression que cet homme est pressé d'aller aux WC...");
                    anim.HideDialog();
                    anim.Execute();
                }
                break;
            case "L'Agent retourne au guichet":
                {
                    anim.Enable("Agent_1");
                    anim.MoveTo("Agent_1", "Agent 1 Walking Path", GameObject.Find("Position Agent 1").transform.position);
                    anim.Execute();
                }
                break;
            case "Faire disparaitre l'objet brillant":
                {
                    GameObject.Find("Pièces de monnaies")?.SetActive(false);
                }
                break;
        }
    }
}
