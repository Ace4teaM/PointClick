using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlleur réalisant le lien entre le Graph du jeu et les animations de la scène en cours
/// </summary>
public class InspectingController : MonoBehaviour
{
    public GameGraph gameGraph;
    public Animations animations;

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
        var anim = animations;
        if (anim?.animationInProgress == true)
        {
            wantSkipAnimation = true;
            return;
        }

        // Valide l'animation en cours
        if (GameData.action == ActionType.Validate)
            return;

        // Vérifie si aucune action
        if (GameData.action == ActionType.None)
            return;

        wantAction = true;
    }

    /// <summary>
    /// Graph précédent
    /// </summary>
    int lastGraph = 0;

    /// <summary>
    /// Etape précédente
    /// </summary>
    char lastStep = char.MinValue;

    /// <summary>
    /// Etape de la dernière action
    /// </summary>
    char prevActionStep = 'A';

    /// <summary>
    /// Etape du dernier Jalon, initialement (S)
    /// </summary>
    char prevBreakpointStep = 'A';
    HashSet<char> validateBreakpointSteps = new HashSet<char>();

    void Start()
    {
        lastStep = char.MinValue;
        lastGraph = gameGraph.graphIndex;
        prevActionStep = 'A';
        prevBreakpointStep = 'A';
    }

    // Update is called once per frame
    void Update()
    {
        var g = gameGraph;
        var anim = animations;

        if (g.isActiveAndEnabled == false)
            return;

        // Essaie d'ignorer l'animation en cours
        if(wantSkipAnimation && anim?.animationInProgress == true)
        {
            anim?.TrySkipAnimation();
            wantSkipAnimation = false;
        }

        // Pas de progression tant que les animations en cours ne sont pas terminées
        if (anim?.animationInProgress == true)
            return;

        // Vérifie si la prochaine étape est une transition automatique
        if (lastStep != g.graphStep || lastGraph != g.graphIndex)
        {
            char nextStep;
            
            lastStep = g.graphStep;
            lastGraph = g.graphIndex;

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
                    var line = g.graphText.Substring(expression.textStart, expression.textEnd - expression.textStart).Trim();
                    var line2 = g.graphText.Substring(nextExpression.textStart, nextExpression.textEnd - nextExpression.textStart).Trim();

                    // Dernière étape, on passe au graph suivant
                    if (nextStep == 'Z' && g.graphIndex + 1 < g.graphs.Count)
                    {
                        g.graphText = g.graphs[++g.graphIndex];
                        g.graphStep = 'A';
                        prevActionStep = 'A';
                        prevBreakpointStep = 'A';
                        validateBreakpointSteps.Clear();
                        return;
                    }

                    // Jalon
                    if (g.TryGetBreakpoint(nextExpression, out var expectedSteps))
                    {
                        validateBreakpointSteps.Add(g.graphStep);
                        // Si toutes les étapes ont été atteintes, on passe à l'étape suivante
                        if (validateBreakpointSteps.SetEquals(expectedSteps))
                        {
                            g.graphStep = nextStep;
                            validateBreakpointSteps.Clear();
                            prevBreakpointStep = nextStep;
                        }
                        // sinon on retourne au jalon précédent
                        else
                        {
                            g.graphStep = prevBreakpointStep;
                        }
                        return;
                    }

                    // Dialogue
                    if (g.TryGetDialog(nextExpression, out var dialog))
                    {
                        if (anim == null)
                        {
                            Debug.LogError($"Impossible de trouver l'objet d'animations 'Animations'");
                        }
                        else
                        {
                            anim.ShowDialog(dialog);
                            anim.HideDialog();
                            anim.start = true;
                        }
                        g.graphStep = nextStep;
                        return;
                    }

                    // Vérifie si la prochaine étape est un choix à plusieurs possibilités
                    if (g.TryGetChoice(nextExpression, out var choiceType))
                    {
                        if (g.IsPrimary)
                        {
                            var newUIScene = String.Empty;

                            if (choiceType == "Actions")
                                newUIScene = "GameUI";
                            else if (choiceType == "Choix")
                                newUIScene = "DialogUI";
                            else if (choiceType == "Trouver")
                                newUIScene = "SearchUI";

                            if (String.IsNullOrEmpty(newUIScene))
                                Debug.LogError($"Impossible de déterminer d'UI pour le type choix: {choiceType}");
                            else if (GameData.CurrentSceneUI != newUIScene)
                                SceneTransition.ChangeUI(newUIScene);
                        }

                        // On passe à l'étape suivante
                        g.graphStep = nextStep;
                        prevActionStep = nextStep; // enregistre la dernière étape d'action pour restaurer si il n'y a pas de suite à l'étape
                        return;
                    }

                    // Vérifie si la prochaine étape est un changement d'état
                    if (g.TryGetState(nextExpression, out var obj, out var state, out var val))
                    {
                        if(val is bool)
                            anim.ChangeState(obj, state, (bool)val);
                        else if(val is float)
                            anim.ChangeState(obj, state, (float)val);
                        else if(val is int)
                            anim.ChangeState(obj, state, (int)val);
                        else
                            Debug.LogError($"Impossible de déterminer un type compatible pour la valeur du changement d'état {obj}.{state}={val}");
                        anim.start = true;
                        // On passe à l'étape suivante
                        g.graphStep = nextStep;
                        return;
                    }

                    if (g.TryGetAnimation(nextExpression, out var animation))
                    {
                        GameData.ShowAnimation = animation;
                        GameData.OnAnimationChange();
                        g.graphStep = nextStep;
                        return;
                    }

                    if (g.TryGetTransition(nextExpression, out var scene, out var initialStates))
                    {
                        anim.Transition(scene, initialStates);
                        anim.start = true;
                        g.graphStep = nextStep;
                        return;
                    }

                    if (g.TryGetItem(nextExpression, out var item))
                    {
                        GameData.AddItem(item);
                        GameData.OnInventoryChange();
                        anim.TriggerAnimator("Inventory","Show");
                        anim.start = true;
                        g.graphStep = nextStep;
                        return;
                    }

                    if (g.TryLoseItem(nextExpression, out var itemLose))
                    {
                        GameData.RemoveItem(itemLose);
                        GameData.OnInventoryChange();
                        anim.TriggerAnimator("Inventory", "Show");
                        anim.start = true;
                        g.graphStep = nextStep;
                        return;
                    }

                    throw new Exception("Etape non gérée");
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
            if (HoverCursorFlagStates.HoverFlagType == HoverFlagType.UI)
                return;

            GameGraph.GraphExpression expression;
            if(
                // utilisation d'un item sur un objet
                (GameData.SelectedInventoryItem != InventoryItem.Empty && g.TryFindUseAction(g.graphStep, GameData.SelectedInventoryItem.label, HoverCursorFlagStates.HoverFlag, out expression))
                ||
                // ou action sur un objet
                (GameData.SelectedInventoryItem == InventoryItem.Empty && g.TryFindAction(g.graphStep, GameData.action, HoverCursorFlagStates.HoverFlag, out expression)))
            {
                Debug.Log(g.graphText.Substring(expression.textStart, expression.textEnd - expression.textStart));

                // d'abord on se déplace vers l'objet
                if (GameData.action == ActionType.Talk || GameData.action == ActionType.Activate)
                {
                    anim.MoveTo("Fred", HoverCursorFlagStates.HoverFlag);
                }

                // examine le résultat de l'action
                if (g.TryGetDialog(expression, out var dialog))
                {
                    anim.ShowDialog(dialog);
                    anim.HideDialog();
                    anim.start = true;
                }
                else if (g.TryGetTransition(expression, out var scene, out var initialStates))
                {
                    anim.Transition(scene, initialStates);
                    anim.start = true;
                }
                else if (g.TryGetItem(expression, out var item))
                {
                    GameData.AddItem(item);
                    GameData.OnInventoryChange();
                }
                else if (g.TryLoseItem(expression, out var itemLose))
                {
                    GameData.RemoveItem(itemLose);
                    GameData.OnInventoryChange();
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
                    // on continue dans le graph
                    g.graphStep = nextStep;

                    // Déselectionne l'objet
                    GameData.SelectedInventoryItem = InventoryItem.Empty;
                    GameData.OnSelectedItemChange();
                }
                else
                {
                    // sinon, on recommence le graph à l'action précédente
                    // (généralement un dialogue sans suite mais pas la fin du graph)
                    g.graphStep = prevActionStep;
                }
            }
        }
    }
}
