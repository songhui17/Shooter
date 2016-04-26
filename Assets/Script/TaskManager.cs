using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    #region Properties

    private static TaskManager _instance;
    public static TaskManager Instance {
        get{ return _instance; }
        set{ _instance = value; }
    }

    [SerializeField]
    private List<Task> _taskList;
    public List<Task> TaskList{
        get { return _taskList ?? (_taskList = new List<Task>()); }
    }

    [SerializeField]
    private List<Task> _pendingTaskList;
    public List<Task> PendingTaskList{
        get { return _pendingTaskList ??
                    (_pendingTaskList = new List<Task>()); }
    }

    #endregion
   
    #region MonoBehaviour 

    void Awake(){
        if( Instance == null){
            Instance = this;
        }else{
            enabled = false;
            Debug.LogWarning("Duplicate instance of TaskManager");
        }
    }

    #endregion

    #region Methods

    public Task GetTask(Actor actor_){
        if (actor_ == null) return null;

        Profiler.BeginSample("GetTask");
        Task task = null;
        for(var idx = 0; idx < TaskList.Count; idx++){
            var task_ = TaskList[idx];
            if (task_.ValidateActor(actor_)){
                task = task_;
                break;
            }
        }

        if (task != null){
            TaskList.Remove(task);
            PendingTaskList.Add(task);
        }
        Profiler.EndSample();
        return task;
    }

    #endregion
}
