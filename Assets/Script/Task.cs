using UnityEngine;
using System;
using System.Collections;

public class Task : MonoBehaviour {

#region Methods

    public virtual bool ValidateActor(Actor actor_){
        if (actor_ == null) return false;
        return true;
    }
    
    public virtual bool ActivateTask(Actor actor_){
        if (actor_ == null) return false;
        return true;
    }

    public void ReportDone(Actor actor_){
        OnTaskDone();
    }

    public virtual bool IsSatisfied(){
        return false;
    }

    public event Action TaskDone;
    protected void OnTaskDone()
    {
        if (TaskDone != null) TaskDone();
    }

#endregion

}
