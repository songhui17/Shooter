using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TaskPlanner : MonoBehaviour {
    protected bool _verbose = true;

    protected Task _activeTask;
    private Bot _bot;
    protected Bot Bot {
        get { return _bot ?? (_bot = GetComponent<Bot>()); }
    }

    protected string status{
        get { return Bot.status; }
        set { Bot.status = value; }
    }

    private string __patrolStatus;
    public string _patrolStatus {
        get { return __patrolStatus; }
        set {
            Debug.Log(string.Format(
                "Change __patrolStatus from {0} -> {1}",
                __patrolStatus, value));

            if (__patrolStatus == "inspecting" &&
                    value != "inspecting"){
                // TODO:
                var patrolTask = _activeTask as PatrolTask;
                if (patrolTask != null){
                    patrolTask.Interrupt();
                }
            }
            __patrolStatus = value;
        }
    }

    void Start(){
        StartCoroutine(TickTaskPlanner());
    }
    
    IEnumerator TickTaskPlanner(){
        var waitForSeconds = new WaitForSeconds(0.1f);
        while (true){
            yield return waitForSeconds;

            Profiler.BeginSample("TickTaskPlanner");
            // Plan to take next action or finish
            TickPlanner();
            Profiler.EndSample();

            // Plan to get some new task to do or continue
            Profiler.BeginSample("GetTask");
            GetTask();
            Profiler.EndSample();
        }
    }

    protected virtual void TickPlanner(){
    }

    protected virtual void GetTask(){
    }

}
