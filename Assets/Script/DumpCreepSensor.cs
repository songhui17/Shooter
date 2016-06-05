using UnityEngine;
using System.Collections;

public class DumpCreepSensor : Sensor {
    void Start() {
        StartCoroutine(CoFindPlayer());
    }

    IEnumerator CoFindPlayer() {
        yield return null;
        while (true) {
            var player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length > 0) {
                AttackTargetList.Clear();
                AttackTargetList.Add(player[0]);
                break;
            }
            yield return null;
        }
    }
}
