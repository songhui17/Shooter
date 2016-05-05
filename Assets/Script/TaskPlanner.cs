using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TaskPlanner : MonoBehaviour {
    private bool _verbose = true;

    private Task _activeTask;
    private Bot _bot;
    private Bot Bot {
        get { return _bot ?? (_bot = GetComponent<Bot>()); }
    }

    private string status{
        get { return Bot.status; }
        set { Bot.status = value; }
    }

    private PatrolAction _patrolAction;
    private string __patrolStatus;
    public string _patrolStatus {
        get { return __patrolStatus; }
        set {
            Debug.Log(string.Format(
                "Change __patrolStatus from {0} -> {1}",
                __patrolStatus, value));

            if (__patrolStatus == "inspecting" &&
                    value != "inspecting"){
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
            if (_activeTask != null){
                if (TickGotoTask(_activeTask)) continue;
                if (TickFarmTask(_activeTask)) continue;
                if (TickPatrolTask(_activeTask)) continue;
            }
            Profiler.EndSample();

            // Plan to get some new task to do or continue
            Profiler.BeginSample("GetTask");
            GetTask();
            Profiler.EndSample();
        }
    }

    void GetTask(){
        if (_activeTask == null){
            _activeTask = TaskManager.Instance.GetTask(Bot);
            if(_activeTask != null)
                Debug.Log(string.Format(
                    "Get _activeTask:{0}", _activeTask));
            status = StartTask(_activeTask);
        }
    }


    #region Task Planner Methods

    private bool StartGotoTask(Task task_){
        var gotoTask = task_ as GotoTask;
        if (gotoTask != null){
            gotoTask.Actor = Bot;
            return true;
        }else{
            return false;
        }
    }

    private bool TickGotoTask(Task task_){
        var gotoTask = task_ as GotoTask;
        if (gotoTask != null){
            if (status != "goto"){
                var gotoTargetPosition = gotoTask.TargetPosition; 
                var gotoDirection = gotoTargetPosition - Bot.transform.position;
                Bot.TakeGotoAction(
                        gotoTargetPosition, 1.0f, gotoDirection, 0.0f, false);
            }

            if (gotoTask.IsSatisfied()){
                OnTaskDone();
            }
            // TODO: handle goto failure
            return true;
        }else{
            return false;
        }
    }

    private bool StartPatrolTask(Task task_){
        if (_patrolAction == null){
            _patrolAction = new PatrolAction(){
                Bot = Bot,
                Sensor = Bot.Sensor,
            };
        }

        // When inspector in Debug Mode, _patrolAction is 
        // created with Bot null
        if (_patrolAction.Bot == null){
            _patrolAction.Bot = Bot;
            _patrolAction.Sensor = Bot.Sensor;
        }

        return _patrolAction.Start(task_);
    }

    private bool TickPatrolTask(Task task_){
        if (_patrolAction.Update(task_)){
            var patrolTask = task_ as PatrolTask;
            if (patrolTask.IsSatisfied()){
                OnTaskDone();
                // TODO: handle patrol failure: bot die
            }
            return true;
        }else{
            return false;
        }
    }

    private bool StartFarmTask(Task task_){
        var farmTask = task_ as FarmTask;
        if (farmTask != null){
            if (farmTask.IsSatisfied()){
                OnTaskDone();
            }
            // TODO: handle farmTask failure
            return true;
        }else{
            return false;
        }
    }

    private bool TickFarmTask(Task task_){
        return false;
    }

    private string StartTask(Task task_){
        _activeTask = task_;
        if (_activeTask == null) return "idle";

        if (StartGotoTask(task_)){
            return "idle"; // use Tick...Task
        }

        if (StartFarmTask(task_)){
            return "idle"; // use Tick...Task
        }

        if (StartPatrolTask(task_)){
            return "idle"; // use Tick...Task
        }

        OnTaskDone();
        return "idle";
    }

    private void OnTaskDone(){
        Debug.Log("There is no more action to take.");

        if (_activeTask.IsSatisfied()){
            Debug.Log(string.Format(
                    "_activeTask:{0} is satisfied.",
                    _activeTask));
            _activeTask = null;
        }else{
            Debug.Log(string.Format(
                    "_activeTask:{0} is failed" + 
                    " after actions are token.",
                    _activeTask));
        }
        status = "idle";
    }

    #endregion
}
