using UnityEngine;
using System;
using System.Collections;

public class GotoTask : Task
{
    private bool _actorInZone = false;

    private Actor _actor;
    public Actor Actor {
       set {
           if (_actor != null && value != _actor)
               throw new Exception("_actor is not Null");
           _actor = value;
       }
    }

    public Vector3 TargetPosition{
        get { return transform.position; }
    }

    public override bool ValidateActor(Actor actor_){
        return true;
    }

    public override bool IsSatisfied(){
        if (_actorInZone){
            // TODO: remove duplicate call
            OnTaskDone();
        }
        return _actorInZone;
    }

    void OnTriggerEnter(Collider collider_)
    {
        Debug.Log(StringUtil.Format("GotoTask OnTriggerEnter collider_:{0}", collider_.name));
        // if (collider_.tag == "Player")
        if (_actor != null && collider_.gameObject == _actor.gameObject)
        {
            // TODO: handle _actor inited in zone
            _actorInZone = true;
            OnTaskDone();
        }
    }
}
