using UnityEngine;
using System;
using System.Collections.Generic;

public class PatrolTask : Task {
    [SerializeField]
    private List<Transform> _patrolPoints = new List<Transform>();

    private int _index = 0;
    public Vector3 NextPatrolPoint{
        get {
            var point = _patrolPoints[_index++].position;
            _index %= _patrolPoints.Count;
            return point;
        }
    }

    public void Interrupt(){
        _index--;
        if (_index < 0) _index = _patrolPoints.Count - 1;
    }

    public override bool ValidateActor(Actor actor_){
        return actor_ as Bot != null;
    }

    public override bool IsSatisfied(){
        return _patrolPoints.Count <= 0;
    }
}
