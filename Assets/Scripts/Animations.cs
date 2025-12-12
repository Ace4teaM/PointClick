using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    void MoveTo(string playerName, string anchorName)
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

    void ChangeState(string playerName, string name, bool value)
    {
        var player = GameObject.Find(playerName);
        tasks.Add(() => WaitForBoolAsync(
            () =>
            {
                player.GetComponentInChildren<Animator>().SetBool(name, value);
            },
            () => player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Sat_S"))
        );
    }

    void ChangeState(string playerName, string name, float value, Func<Task> delay)
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
                    MoveTo("Fred", "A_Bibliotheque");
                    MoveTo("Fred", "A_Canape");
                    ChangeState("Fred", "IsSat", true);
                    ChangeState("Fred", "SatState", 2f, () => Task.Delay(3000));
                    ChangeState("Fred", "IsSat", false);
                    start = true;
                }
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public bool start = false;

    void Update()
    {
        if (start)
        {
            RunAll();
            start = false;
        }
    }

    private async void RunAll()
    {
        foreach (var f in tasks)
        {
            await f(); 
        }
    }
}
