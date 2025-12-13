using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Animations : MonoBehaviour
{
    List<Func<Task>> tasks = new List<Func<Task>>();

    void Awake()
    {
        GameData.OnAnimationChanged += OnAnimate;
    }
    void OnDestroy()
    {
        GameData.OnAnimationChanged -= OnAnimate;
    }

    public async Task WaitForBoolAsync(Action exec, Func<bool> condition)
    {
        exec();
        // Attend que la condition soit vraie
        while (!condition())
        {
            await Task.Delay(10); // petite pause pour éviter de bloquer le CPU
        }
    }

    public async Task WaitForBoolAsync(Action exec, Func<Task> condition)
    {
        exec();
        var task = condition();
        // Attend que la condition soit vraie
        while (!task.IsCompleted)
        {
            await Task.Delay(10); // petite pause pour éviter de bloquer le CPU
        }
    }

    internal void Transition(Scenes scene)
    {
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                SceneTransition.SetTransition(scene);
            },
            () => SceneTransition.loading == false)
        );
    }

    internal void ShowDialog(string dialog, Func<Task> delay)
    {
        var obj = GameObject.Find("Dialog");
        if (obj == null)
        {
            Debug.LogError("Impossible de trouver l'objet 'Dialog' pour afficher le texte");
            return;
        }
        var text = obj.GetComponentInChildren<TextMeshProUGUI>();
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                text.text = dialog;
            },
            delay)
        );
    }

    internal void HideDialog()
    {
        var obj = GameObject.Find("Dialog");
        if (obj == null)
        {
            Debug.LogError("Impossible de trouver l'objet 'Dialog' pour afficher le texte");
            return;
        }
        var text = obj.GetComponentInChildren<TextMeshProUGUI>();
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                text.text = String.Empty;
            },
            () => true)
        );
    }

    internal void MoveTo(string playerName, string anchorName)
    {
        var anchor = GameObject.Find(anchorName).GetComponentInChildren<Transform>();
        var player = GameObject.Find(playerName);
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                var path = GameObject.Find("MovingController").GetComponentInChildren<MovingController>().MakePath(anchor.position);
                player.GetComponentInChildren<MoverAnimator>().SetDestinations(path);
            },
            () => player.GetComponentInChildren<MoverAnimator>().IsFinish)
        );
    }

    internal void ChangeState(string playerName, string name, bool value)
    {
        var player = GameObject.Find(playerName);
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetBool(name, value);
            },
            () => true)
        );
    }

    internal void ChangeState(string playerName, string name, float value)
    {
        var player = GameObject.Find(playerName);
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetFloat(name, value);
            },
            () => true)
        );
    }

    internal void ChangeState(string playerName, string name, int value)
    {
        var player = GameObject.Find(playerName);
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetInteger(name, value);
            },
            () => true)
        );
    }

    internal void ChangeState(string playerName, string name, float value, Func<Task> delay)
    {
        var player = GameObject.Find(playerName);
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetFloat(name, value);
                delay();
            },
            delay)
        );
    }

    // Cette fonction sera bindée dans Input Action
    internal void OnAnimate(string animationName)
    {
        switch (animationName)
        {
            case "Fred se lève du canapé":
                {
                    ChangeState("Fred", "IsSat", false);
                    MoveTo("Fred", "A_Canape");
                    start = true;
                }
                break;
            case "Les boites tombent sur Fred":
                {
                    GameObject.Find("Fred").transform.position = GameObject.Find("A_Bibliotheque").transform.position;
                    ChangeState("Fred", "IsDizzy", true);
                    start = true;
                }
                break;
            case "Animation du tonnerre":
                {
                }
                break;
        }
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
        }
        tasks.Clear();
        animationInProgress = false;
    }
}
