using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FarmTask : Task {
    [SerializeField]
    private List<Vector3> _farmSpots = new List<Vector3>();

    public override bool IsSatisfied(){
        return _farmSpots.Count <= 0;
    }
}
