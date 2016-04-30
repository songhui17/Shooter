using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Tricky change to stop patrol task after reach last patrol point
 * 2016/05/01
 * 
 * Use _finishOnLastPoint
 * with _finishOnLastPoint = true:
 *
 * + NextPatrolPoint:
 *   (1) increase index without wrap
 *   (2) return the last patrol point without wrap to the first
 *
 * + IsSatisfied:
 *   (1) return true is no patrol points
 *   (2) return true if last patrol point is reached
 *       by checking _index > patrolPoints.Count
 *
 **/ 
public class PatrolTask : Task {

    [SerializeField]
    private bool _finishOnLastPoint = false;

    [SerializeField]
    private List<Transform> _patrolPoints = new List<Transform>();

    private int _index = 0;
    public Vector3 NextPatrolPoint{
        get {
            if (_finishOnLastPoint){
                if (_index < _patrolPoints.Count)
                    return _patrolPoints[_index++].position;
                else{
                    _index++;
                    return _patrolPoints[_patrolPoints.Count - 1].position;
                }
            }else{
                var point = _patrolPoints[_index++].position;
                _index %= _patrolPoints.Count;
                return point;
            }
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
        if (_finishOnLastPoint){
            // (1)
            // _patrolPoints.Count = 0
            // return true
            //
            // (2)
            // i)
            // IsSatisfied:
                // _patrolPoints.Count = 1
                // _index = 0
                // return false;
            // NrxtPatrolPoint:
                // return _patrolPoints[0]
                // _index = 1
            // IsSatisfied:
                // _index = 1
                // _patrolPoints.Count = 1
                // return false
            //
            // ii)
            // NextPatrolPoint:
                // return _patrolPoints[0]
                // _index = 2
            // IsSatisfied:
                // _index = 2
                // _patrolPoints.Count = 1
                // return true
            return _patrolPoints.Count <= 0 || _patrolPoints.Count < _index;
        }else{
            return _patrolPoints.Count <= 0;
        }
    }
}
