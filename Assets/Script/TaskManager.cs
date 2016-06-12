using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    #region Properties
    [SerializeField]
    private bool _verbose = true;

    private static TaskManager _instance;
    public static TaskManager Instance {
        get{ return _instance; }
        set{ _instance = value; }
    }

    [SerializeField]
    private FightFinishViewModel _fightFinishViewModel; //= FightFinishViewModel.Instance;
    
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

        foreach (var task in TaskList){
            if (task.IsPrimaryTask){
                task.TaskDone += () => {
                    if (_verbose)
                        Debug.Log(string.Format(
                            "TaskDone: {0}, _fightFinishViewModel: {1}",
                            task, _fightFinishViewModel != null
                                ? _fightFinishViewModel.ToString() : "null"));
                    if (_fightFinishViewModel != null){
                        _fightFinishViewModel.IsFightFinished = true;
                    }else{
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    }
                };
            }
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
