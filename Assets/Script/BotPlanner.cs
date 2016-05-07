using UnityEngine;

public class BotPlanner : TaskPlanner {

    private PatrolAction _patrolAction;

    protected override void TickPlanner(){
        if (_activeTask != null){
            if (TickGotoTask(_activeTask)) return;
            if (TickPatrolTask(_activeTask)) return;
        }
    }

    protected override void GetTask(){
        if (_activeTask == null){
            _activeTask = TaskManager.Instance.GetTask(Bot);
            if(_activeTask != null)
                Debug.Log(string.Format(
                    "Get _activeTask:{0}", _activeTask));
            status = StartTask(_activeTask);
        }
    }

    private string StartTask(Task task_){
        _activeTask = task_;
        if (_activeTask == null) return "idle";

        if (StartGotoTask(task_)){
            return "idle"; // use Tick...Task
        }

        if (StartPatrolTask(task_)){
            return "idle"; // use Tick...Task
        }

        OnTaskDone();
        return "idle";
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
