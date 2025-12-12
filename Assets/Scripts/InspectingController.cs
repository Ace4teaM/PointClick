using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InspectingController : MonoBehaviour
{
    /// <summary>
    /// true Si l'utiliseur a cliqué pendant l'animation pour demander de la passer
    /// </summary>
    private bool wantSkipAnimation = false;

    /// <summary>
    /// true Si l'utiliseur a cliqué pour déplacer l'objet
    /// Cette propriété est utilisée en décalage avec OnClick et Update pour permettre à Unity de calculer toutes les propriétés d'UI avant l'action (ie: EventSystem.current.IsPointerOverGameObject())
    /// </summary>
    private bool wantAction = false;

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
        // Pas de progression tant que les animations en cours ne sont pas terminées
        var anim = GameObject.Find("Animations")?.GetComponent<Animations>();
        if (anim?.animationInProgress == true)
        {
            wantSkipAnimation = true;
            return;
        }

        // Valide l'animation en cours
        if (GameData.action == ActionType.Validate)
            return;

        // Vérifie si l'action en cours est "Inspecter", "Parler" ou "Actionner"
        if (GameData.action != ActionType.Inspect && GameData.action != ActionType.Talk && GameData.action != ActionType.Activate)
            return;

        wantAction = true;
    }

    /// <summary>
    /// Etape précédente
    /// </summary>
    char lastStep = char.MinValue;

    /// <summary>
    /// Etape de la dernière action
    /// </summary>
    char prevActionStep = 'A';

    void Start()
    {
        lastStep = char.MinValue;
    }

    // Update is called once per frame
    void Update()
    {
        var g = GameGraph.Instance;
        var anim = GameObject.Find("Animations")?.GetComponent<Animations>();

        // Essaie d'ignorer l'animation en cours
        /*if(wantSkipAnimation && anim?.animationInProgress == true)
        {
            anim?.TrySkipAnimation();
        }*/

        // Pas de progression tant que les animations en cours ne sont pas terminées
        if (anim?.animationInProgress == true)
            return;

        // Vérifie si la prochaine étape est une transition automatique
        if (lastStep != g.graphStep)
        {
            char nextStep;
            
            lastStep = g.graphStep;

            // Obtient la prochaine étape
            if (g.TryFindImmediateStep(g.graphStep, out var expression))
            {
                // Vérifie si la prochaine étape est une animation
                if (g.TryGetWaitAnimation(expression, out nextStep, out var duration))
                {
                    // On passe à l'étape suivante
                    g.graphStep = nextStep;
                    return;
                }
                else if (g.TryGetNextStep(expression, out nextStep, out var nextExpression))
                {
                    // Dernière étape, on passe au graph suivant
                    if (nextStep == 'Z' && g.graphIndex + 1 < g.graphs.Count)
                    {
                        g.graphText = g.graphs[++g.graphIndex];
                        g.graphStep = 'A';
                        return;
                    }

                    // Dialogue
                    if (g.TryGetDialog(nextExpression, out var dialog))
                    {
                        //var obj = SceneUtils.GetObjectByName(GameData.CurrentSceneUI, "Animations");
                        if (anim == null)
                        {
                            Debug.LogError($"Impossible de trouver l'objet d'animations 'Animations'");
                        }
                        else
                        {
                            anim.ShowDialog(dialog, () => Task.Delay(5000));
                            anim.HideDialog();
                            anim.start = true;
                        }
                        g.graphStep = nextStep;
                        return;
                    }

                    // Vérifie si la prochaine étape est un choix à plusieurs possibilités
                    if (g.TryGetChoice(nextExpression))
                    {
                        // On passe à l'étape suivante
                        g.graphStep = nextStep;
                        prevActionStep = nextStep; // enregistre la dernière étape d'action pour restaurer si il n'y a pas de suite à l'étape
                        return;
                    }

                    if (g.TryGetAnimation(nextExpression, out var animation))
                    {
                        GameData.ShowAnimation = animation;
                        GameData.OnAnimationChange();
                        g.graphStep = nextStep;
                        return;
                    }
                }
            }
            // si il n'y a pas de prochaine étape, on recommence l'action précédente
            else
            {
                if (g.HasNextStep(g.graphStep) == false)
                {
                    // (généralement un dialogue sans suite mais pas la fin du graph)
                    g.graphStep = prevActionStep;
                }
            }
        }

        // Execute la prochaine action utilisateur
        if (wantAction)
        {
            char nextStep;
            wantAction = false;
            // Le clic vient de l’UI (Button ou autre)
            if (HoverCursorFlag.HoverFlagType == HoverFlagType.UI)
                return;

            if(g.TryFindAction(g.graphStep, GameData.action, HoverCursorFlag.HoverFlag, out var expression))
            {
                Debug.Log(g.graphText.Substring(expression.textStart, expression.textEnd - expression.textStart));

                // examine le résultat de l'action
                if (g.TryGetDialog(expression, out var dialog))
                {
                    anim.ShowDialog(dialog, () => Task.Delay(5000));
                    anim.HideDialog();
                    anim.start = true;
                }
                else if (g.TryGetTransition(expression, out var scene))
                {
                    if(EnumExtensions.TryParseFromDescription<Scenes>(scene, true, out var sceneType))
                    {
                        anim.Transition((Scenes)sceneType);
                        anim.start = true;
                    }
                    else
                    {
                        Debug.LogError($"Impossible de déterminer la scène de transition ({scene}) de l'action {GameData.action} à l'étape {g.graphStep}");
                        return;
                    }
                }
                else
                {
                    Debug.LogError($"Impossible de déterminer l'action {GameData.action} à l'étape {g.graphStep}");
                    return;
                }

                // Si l'étape actuelle a une suite alors on continue dans le graph
                nextStep = g.graphText[expression.textStart];
                if (g.HasNextStep(nextStep))
                {
                    g.graphStep = nextStep;
                }
                else
                {
                    // sinon, on recommence le graph au début
                    // (généralement un dialogue sans suite mais pas la fin du graph)
                    g.graphStep = prevActionStep;
                }
            }
        }
    }
}
