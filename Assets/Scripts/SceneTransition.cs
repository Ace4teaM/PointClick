using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Description = System.ComponentModel.DescriptionAttribute;

/// <summary>
/// Demande la transition de la scène
/// </summary>
/// <param name="name">Nom de la nouvelle scène</param>
/// <remarks>Les descriptions sont utilisées pour parser les actions du flow-graph</remarks>
public enum Scenes
{
    [Description("Bibliothèque")]
    Bibliotheque,
    [Description("Boites au sol")]
    BoitesAuSol,
    [Description("Pièce principale")]
    PiecePrincipale
}

public class SceneTransition : MonoBehaviour
{
    internal static bool loading = false;
    internal static bool loadTransition = false;
    internal static float fadeInTimer = 0f;
    internal static float fadeOutTimer = 0f;
    internal static string newCurrentSceneGame = string.Empty;
    internal static string newCurrentSceneUI = string.Empty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (loadTransition)
        {
            StartCoroutine(LoadAndUnload(GameData.TransitionScene, GameData.CurrentSceneUI, newCurrentSceneUI, GameData.CurrentSceneGame, newCurrentSceneGame));
            loadTransition = false;
        }
    }

    internal static void ChangeUI(string sceneUI)
    {
        if (loadTransition == true)
            return;

        newCurrentSceneGame = GameData.CurrentSceneGame;
        newCurrentSceneUI = sceneUI;
        GameData.TransitionScene = null;

        loadTransition = true;
    }

    internal static void SetTransition(Scenes scene)
    {
        if (loadTransition == true)
            return;
           
        newCurrentSceneGame = GameData.CurrentSceneGame;
        newCurrentSceneUI = GameData.CurrentSceneUI;
        GameData.TransitionScene = "CircleTransition";

        switch (scene)
        {
            case Scenes.Bibliotheque:
                newCurrentSceneGame = "Bibliotheque";
                newCurrentSceneUI = "SearchUI";
                break;
            case Scenes.BoitesAuSol:
                newCurrentSceneGame = "Boites au sol";
                newCurrentSceneUI = "SearchUI";
                break;
            case Scenes.PiecePrincipale:
                newCurrentSceneGame = "Piece Principale";
                newCurrentSceneUI = "GameUI";
                break;
        }

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
    private IEnumerator LoadAndUnload(string transitionName, string oldCurrentSceneUI, string newCurrentSceneUI, string oldCurrentSceneGame, string newCurrentSceneGame)
    {
        fadeOutTimer = 0f;
        fadeInTimer = 0f;

        // Décharge l'UI (même si la nouvelle est identique)
        yield return SceneManager.UnloadSceneAsync(oldCurrentSceneUI);

        // Charge la scène de transition
        if (transitionName != null)
        {
            yield return SceneManager.LoadSceneAsync(transitionName, LoadSceneMode.Additive);

            yield return WaitAndAnimate(2f, t =>
            {
                fadeInTimer = t;
            });
        }

        // Charger la scène de jeu (si nécessaire)
        if (newCurrentSceneGame != oldCurrentSceneGame && SceneManager.GetSceneByName(oldCurrentSceneGame).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(oldCurrentSceneGame);
            AsyncOperation op = SceneManager.LoadSceneAsync(newCurrentSceneGame, LoadSceneMode.Additive);
            while (!op.isDone)
                yield return null;
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
        yield return SceneManager.LoadSceneAsync(newCurrentSceneUI, LoadSceneMode.Additive);

        loading = false;

        // NOTE: initialisé automatiquement par le script InitStates
        //GameData.CurrentSceneGame = newCurrentSceneGame;
        //GameData.CurrentSceneUI = newCurrentSceneUI;
    }
}
