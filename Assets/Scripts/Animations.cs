using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Animations : MonoBehaviour
{
    List<Func<Task>> tasks = new List<Func<Task>>();
    CancellationTokenSource cancel = new CancellationTokenSource();

    public AnimationAsset animationAsset;

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
#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    void OnPlayModeChanged(PlayModeStateChange state)
    {
        // s'assure que les taches sont annulées à la fin du mode Play
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            cancel.Cancel();
            tasks.Clear();
        }
    }
#endif
    public void Execute()
    {
        start = true;
    }

    public void TrySkipAnimation()
    {
        cancel.Cancel();
    }

    public async Task ExecuteAndWaitForBoolAsync(Action exec, Func<bool> condition, bool cancelable)
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

    public async Task ExecuteAndWaitForBoolAsync(Action exec, Func<Task> condition, bool cancelable)
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

    public async Task RepeatAndWaitForBoolAsync(Action exec, Func<bool> condition, bool cancelable)
    {
        // Attend que la condition soit vraie
        while (!condition())
        {
            exec();
            if (cancelable && cancel.IsCancellationRequested)
                break;
            await Task.Delay(10); // petite pause pour éviter de bloquer le CPU
        }
    }

    public async Task RepeatAndWaitForBoolAsync(Action exec, Func<Task> condition, bool cancelable)
    {
        var task = condition();
        // Attend que la condition soit vraie
        while (!task.IsCompleted)
        {
            exec();
            if (cancelable && cancel.IsCancellationRequested)
                break;
            await Task.Delay(10); // petite pause pour éviter de bloquer le CPU
        }
    }

    internal void Wait(int milliseconds)
    {
        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () => { },
            () => Task.Delay(milliseconds),
            true)
        );
    }

    internal void Transition(string scene)
    {
        tasks.Add(() => ExecuteAndWaitForBoolAsync(
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
        tasks.Add(() => ExecuteAndWaitForBoolAsync(
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

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
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
        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () =>
            {
                GameData.ShowDialog = String.Empty;
                GameData.OnDialogChange();
            },
            () => true, 
            false)
        );
    }

    internal void Disable(string objectName)
    {
        var obj = Animations.FindInactiveInScenes(objectName);

        if (obj == null)
            throw new Exception($"Object name '{objectName}' not found in game objects");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () =>
            {
                obj.SetActive(false);
            },
            () => true,
            false)
        );
    }

    internal void Enable(string objectName)
    {
        var obj = Animations.FindInactiveInScenes(objectName);

        if (obj == null)
            throw new Exception($"Object name '{objectName}' not found in game objects");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () =>
            {
                obj.SetActive(true);
            },
            () => true,
            false)
        );
    }

    internal void MoveTo(string playerName, string anchorName)
    {
        var anchor = Animations.FindInactiveInScenes(anchorName)?.GetComponentInChildren<Transform>();
        var player = Animations.FindInactiveInScenes(playerName);

        if (anchor == null)
            throw new Exception($"Object name '{anchorName}' not found in game objects");

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () =>
            {
                var path = GameObject.Find("MovingController").GetComponentInChildren<MovingController>().MakePath(anchor.position);
                player.GetComponentInChildren<MoverAnimator>().SetDestinations(path);
            },
            () => player.GetComponentInChildren<MoverAnimator>().IsFinish,
            false)
        );
    }

    internal void MoveTo(string playerName, string walkingPathName, Vector3 position)
    {
        var player = Animations.FindInactiveInScenes(playerName);
        var walkingPath = Animations.FindInactiveInScenes(walkingPathName)?.GetComponent<PathFinder>();

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        if (walkingPath == null)
            throw new Exception($"Impossible trouver l'objet : {walkingPathName}");

        var path = walkingPath.FindPath(player.transform.position, position);

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<MoverAnimator>().SetDestinations(path);
            },
            () => player.GetComponentInChildren<MoverAnimator>().IsFinish,
            false)
        );
    }

    internal void ChangeProperty<T>(string objectName, string propertyName, object value) where T : Component
    {
        var obj = Animations.FindInactiveInScenes(objectName);

        if (obj == null)
            throw new Exception($"Object '{objectName}' not found in game objects");

        var comp = obj.GetComponentInChildren<T>();

        if (comp == null)
            throw new Exception($"Object '{objectName}' as not component {typeof(T).Name}");

        var prop = comp.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

        if (prop == null)
            throw new Exception($"Object '{objectName}' as not property {propertyName}");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () =>
            {
                prop.SetValue(comp, value);
            },
            () => true,
            false)
        );
    }

    internal void UpTo<T>(string objectName, string propertyName, float delta, float targetValue) where T : Component
    {
        var obj = Animations.FindInactiveInScenes(objectName);

        if (obj == null)
            throw new Exception($"Object '{objectName}' not found in game objects");

        var comp = obj.GetComponentInChildren<T>();

        if (comp == null)
            throw new Exception($"Object '{objectName}' as not component {typeof(T).Name}");

        var prop = comp.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

        if (prop == null)
            throw new Exception($"Object '{objectName}' as not property {propertyName}");

        tasks.Add(() => RepeatAndWaitForBoolAsync(
            () =>
            {
                prop.SetValue(comp, (float)prop.GetValue(comp) + delta * Time.deltaTime);
            },
            () => comp == null || Math.Abs((float)prop.GetValue(comp) - targetValue) < Math.Abs(delta * Time.deltaTime),
            false)
        );
    }

    internal void ChangeState(string playerName, string name, bool value)
    {
        var player = Animations.FindInactiveInScenes(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
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
        var player = Animations.FindInactiveInScenes(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
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
        var player = Animations.FindInactiveInScenes(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
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
        var player = Animations.FindInactiveInScenes(playerName);

        if (player == null)
            throw new Exception($"Player name '{playerName}' not found in game objects");

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
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
        for(int i=0;i<SceneManager.sceneCount;i++)
        {
            var scene = SceneManager.GetSceneAt(i);
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
        animationAsset?.OnAnimate(this, animationName);
    }

    /// <summary>
    /// Déclenche une animation
    /// </summary>
    /// <param name="objectName">Nom de l'objet possédant un composant 'Animator'</param>
    /// <param name="triggerName">Nom du trigger de l'animation</param>
    internal void TriggerAnimator(string objectName, string triggerName, string? resetName)
    {
        var obj = GameObject.Find(objectName);

        if (obj == null)
            return;

        tasks.Add(() => ExecuteAndWaitForBoolAsync(
            () =>
            {
                if(resetName != null)
                    obj.GetComponentInChildren<Animator>().ResetTrigger(resetName);
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
            _ = RunAllAsync(); // fire-and-forget maîtrisé
            start = false;
        }
    }

    private async Task RunAllAsync()
    {
        animationInProgress = true;

        try
        {
            foreach (var f in tasks)
            {
                try
                {
                    var task = f();

                    await task;
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }

                if (cancel.IsCancellationRequested)
                    cancel = new CancellationTokenSource();
            }
        }
        finally
        {
            tasks.Clear();
            animationInProgress = false;
        }
    }
}
