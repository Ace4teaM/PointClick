using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Animations : MonoBehaviour
{
    List<Func<Task>> tasks = new List<Func<Task>>();
    CancellationTokenSource cancel = new CancellationTokenSource();

    private static int readTimeByWordInMs = 300;
    private static int readTimeMinMs = 1000 * 2;
    private static int readTimeMaxMs = 1000 * 10;

    void Awake()
    {
        GameData.OnAnimationChanged += OnAnimate;
    }
    void OnDestroy()
    {
        GameData.OnAnimationChanged -= OnAnimate;
    }

    public void TrySkipAnimation()
    {
        cancel.Cancel();
    }

    public async Task WaitForBoolAsync(Action exec, Func<bool> condition, bool cancelable)
    {
        exec();
        // Attend que la condition soit vraie
        while (!condition())
        {
            if (cancelable && cancel.IsCancellationRequested)
                break;
            await Task.Delay(10); // petite pause pour éviter de bloquer le CPU
        }
    }

    public async Task WaitForBoolAsync(Action exec, Func<Task> condition, bool cancelable)
    {
        exec();
        var task = condition();
        // Attend que la condition soit vraie
        while (!task.IsCompleted)
        {
            if (cancelable && cancel.IsCancellationRequested)
                break;
            await Task.Delay(10); // petite pause pour éviter de bloquer le CPU
        }
    }

    internal void Transition(string scene)
    {
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                SceneTransition.SetTransition(scene, null);
            },
            () => SceneTransition.loading == false
            , false)
        );
    }

    internal void Transition(string scene, string initialStates)
    {
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                SceneTransition.SetTransition(scene, initialStates);
            },
            () => SceneTransition.loading == false
            , false)
        );
    }

    internal void ShowDialog(string dialog, Func<Task> delay = null)
    {
        // estime le temps en fonction du nombre de mots (10s max, 2s mini)
        if (delay == null)
        {
            var duration = Math.Max(readTimeMinMs, Math.Min(readTimeMaxMs, readTimeByWordInMs * dialog.Count(c => c == ' ')));
            delay = () => Task.Delay(duration);
        }

        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                GameData.ShowDialog = dialog;
                GameData.OnDialogChange();
            },
            delay,
            true)
        );
    }

    internal void HideDialog()
    {
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                GameData.ShowDialog = String.Empty;
                GameData.OnDialogChange();
            },
            () => true, 
            false)
        );
    }

    internal void MoveTo(string playerName, string anchorName)
    {
        var anchor = GameObject.Find(anchorName)?.GetComponentInChildren<Transform>();
        var player = GameObject.Find(playerName);

        if (anchor == null)
            throw new Exception($"Object name '{anchorName}' not found in game objects");

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                var path = GameObject.Find("MovingController").GetComponentInChildren<MovingController>().MakePath(anchor.position);
                player.GetComponentInChildren<MoverAnimator>().SetDestinations(path);
            },
            () => player.GetComponentInChildren<MoverAnimator>().IsFinish,
            false)
        );
    }

    internal void ChangeState(string playerName, string name, bool value)
    {
        var player = GameObject.Find(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetBool(name, value);
            },
            () => true,
            false)
        );
    }

    internal void ChangeState(string playerName, string name, float value)
    {
        var player = GameObject.Find(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetFloat(name, value);
            },
            () => true,
            false)
        );
    }

    internal void ChangeState(string playerName, string name, int value)
    {
        var player = GameObject.Find(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetInteger(name, value);
            },
            () => true,
            false)
        );
    }

    internal void ChangeState(string playerName, string name, float value, Func<Task> delay)
    {
        var player = GameObject.Find(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetFloat(name, value);
                delay();
            },
            delay,
            false)
        );
    }

    /// Recherche un enfant désactivé (avec SetActive) qui ne peut êtrer trouvé avec les méthodes classiques (GameObject.Find...)
    /// Recherche dans toute la scène
    internal static GameObject FindInactiveInScenes(string name)
    {
        foreach (var scene in SceneManager.GetAllScenes())
        {
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                if (root.name == name)
                    return root.gameObject;

                GameObject result = FindInactive(root.transform, name);
                if (result != null)
                    return result;
            }
        }
        return null;
    }

    /// Recherche un enfant désactivé (avec SetActive) qui ne peut êtrer trouvé avec les méthodes classiques (GameObject.Find...)
    /// Recherche dans un parent
    internal static GameObject FindInactive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject result = FindInactive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }


    // Cette fonction sera bindée dans Input Action
    internal void OnAnimate(string animationName)
    {
        switch (animationName)
        {
            case "Fred se lève du canapé":
                {
                    ChangeState("Fred", "IsSat", false);
                    MoveTo("Fred", "Canapé");
                    start = true;
                }
                break;
            case "Les boites tombent sur Fred":
                {
                    GameObject.Find("Fred").transform.position = GameObject.Find("Bibliothèque").transform.position;
                    ChangeState("Fred", "IsDizzy", true);
                    start = true;
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
                    FindInactiveInScenes("Agent")?.SetActive(true);
                    FindInactiveInScenes("Agent_1")?.SetActive(true);
                }
                break;
            default:
                throw new Exception($"Impossible de déterminer l'animation nommée: '{animationName}'");
        }
    }

    /// <summary>
    /// Déclenche une animation
    /// </summary>
    /// <param name="objectName">Nom de l'objet possèdant un composant 'Animator'</param>
    /// <param name="triggerName">Nom du trigger de l'animation</param>
    internal void TriggerAnimator(string objectName, string triggerName)
    {
        var obj = GameObject.Find(objectName);

        if (obj == null)
            return;

        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                obj.GetComponentInChildren<Animator>().SetTrigger(triggerName);
            },
            () => true/*player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Entry") == false*/,
            false)
        );
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    internal bool start = false;
    internal bool animationInProgress = false;

    void Update()
    {
        if (start && animationInProgress == false)
        {
            RunAll();
            start = false;
        }
    }

    private async void RunAll()
    {
        animationInProgress = true;
        foreach (var f in tasks)
        {
            await f();

            // si une annulation a eu lieu, on restore l'instance
            if (cancel.IsCancellationRequested)
                cancel = new CancellationTokenSource();
        }
        tasks.Clear();
        animationInProgress = false;
    }
}
