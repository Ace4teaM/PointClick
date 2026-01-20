using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    internal static bool loading = false;
    internal static bool loadTransition = false;
    internal static float fadeInTimer = 0f;
    internal static float fadeOutTimer = 0f;
    internal static string newCurrentSceneGame = string.Empty;
    internal static string newCurrentSceneUI = string.Empty;
    internal static string newInitialStates = string.Empty;
    internal static Action newCallback = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (loadTransition)
        {
            StartCoroutine(LoadAndUnload(GameData.TransitionScene, newInitialStates, GameData.CurrentSceneUI, newCurrentSceneUI, GameData.CurrentSceneGame, newCurrentSceneGame, newCallback));
            loadTransition = false;
        }
    }

    internal static void ChangeUI(string sceneUI)
    {
        if (loadTransition == true)
            return;

        if (GameData.CurrentSceneUI == sceneUI)
            return;

        newCurrentSceneGame = GameData.CurrentSceneGame;
        newCurrentSceneUI = sceneUI;
        GameData.TransitionScene = null;

        loadTransition = true;
    }

    internal static void SetTransition(string scene, string initialStates)
    {
        if (loadTransition == true)
            return;

        newCurrentSceneGame = scene;
        newCurrentSceneUI = GameData.CurrentSceneUI;
        newInitialStates = initialStates;
        GameData.TransitionScene = "CircleTransition";

        loading = true;

        loadTransition = true;
    }

    internal static void SetTransition(string scene, string ui, string initialStates, Action callback)
    {
        if (loadTransition == true)
            return;

        newCurrentSceneGame = scene;
        newCurrentSceneUI = ui;
        newInitialStates = initialStates;
        newCallback = callback;
        GameData.TransitionScene = "CircleTransition";

        loading = true;

        loadTransition = true;
    }

    /// <summary>
    /// Coroutine générique qui appelle un callback à chaque frame
    /// </summary>
    /// <param name="duration">Durée en secondes</param>
    /// <param name="onUpdate">callback</param>
    IEnumerator WaitAndAnimate(float duration, Action<float> onUpdate)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration); // 0 -> 1
            onUpdate?.Invoke(t); // callback pour animer

            yield return null; // attend la prochaine frame
        }

        onUpdate?.Invoke(1f); // valeur finale
    }

    /// <summary>
    /// Réalise le chargement et déchargement des scènes
    /// </summary>
    /// <param name="transitionName">Nom de la scèen de transition (dossier Transition)</param>
    /// <param name="oldCurrentSceneUI">Nom de la scène UI actuelle</param>
    /// <param name="newCurrentSceneUI">Nom de la scène UI à charger</param>
    /// <param name="oldCurrentSceneGame">Nom de la scène Game actuelle</param>
    /// <param name="newCurrentSceneGame">Nom de la scène Game à charger</param>
    /// <remarks>La scène UI est toujours déchargé pour la faire dispartaitre durant la transition</remarks>
    private IEnumerator LoadAndUnload(string transitionName, string initialStates, string oldCurrentSceneUI, string newCurrentSceneUI, string oldCurrentSceneGame, string newCurrentSceneGame, Action newCallback)
    {
        fadeOutTimer = 0f;
        fadeInTimer = 0f;

        // Charge la scène de transition
        if (transitionName != null)
        {
            yield return SceneManager.LoadSceneAsync(transitionName, LoadSceneMode.Additive);

            yield return WaitAndAnimate(2f, t =>
            {
                fadeInTimer = t;
            });
        }

        // Décharge l'UI (même si la nouvelle est identique)
        if (oldCurrentSceneUI != null)
            yield return SceneManager.UnloadSceneAsync(oldCurrentSceneUI);

        // Charger la scène de jeu (si nécessaire)
        if (newCurrentSceneGame != oldCurrentSceneGame)
        {
            if(oldCurrentSceneGame != null && SceneManager.GetSceneByName(oldCurrentSceneGame).isLoaded)
                yield return SceneManager.UnloadSceneAsync(oldCurrentSceneGame);
            AsyncOperation op = SceneManager.LoadSceneAsync(newCurrentSceneGame, LoadSceneMode.Additive);
            while (!op.isDone)
                yield return null;
        }

        // Etat initial
        if (String.IsNullOrEmpty(initialStates) == false)
        {
            var initStates = SceneUtils.GetObjectByName(newCurrentSceneGame, "InitStates")?.GetComponent<InitStates>();
            if (initStates != null)
            {
                initStates.RestoreState(initStates.states[initialStates]);
            }
        }

        if (transitionName != null)
        {
            yield return WaitAndAnimate(2f, t =>
            {
                fadeOutTimer = t;
            });

            // Unload de la scène de transition
            yield return SceneManager.UnloadSceneAsync(transitionName);
        }

        // Recharge la nouvelle UI
        if (newCurrentSceneUI != null)
            yield return SceneManager.LoadSceneAsync(newCurrentSceneUI, LoadSceneMode.Additive);

        // Callback de fin
        newCallback?.Invoke();

        loading = false;

        // NOTE: initialisé automatiquement par le script InitStates
        //GameData.CurrentSceneGame = newCurrentSceneGame;
        //GameData.CurrentSceneUI = newCurrentSceneUI;
    }
}
