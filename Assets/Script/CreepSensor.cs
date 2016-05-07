using UnityEngine;

public class CreepSensor : Sensor {
    void OnTriggerEnter(Collider collider_){
        Debug.Log(string.Format(
                    "OnTriggerEnter collider_.gameObject: {0}",
                    collider_.gameObject));
        if (collider_.tag == "Player"){
            var target = collider_.gameObject;
            if (!AttackTargetList.Contains(target)){
                var info = string.Format(
                    "Add new attack target {0}" + 
                    "to AttackTargetList(CurrentCount:{1})",
                    target, AttackTargetList.Count);
                Debug.Log(info);
                AttackTargetList.Add(target);
            }else{
                var info = string.Format(
                    "Attack target {0} is already" + 
                    "in AttackTargetList(CurrentCount:{1})",
                    target, AttackTargetList.Count);
                Debug.Log(info);
            }
        }
    }

    void OnTriggerExit(Collider collider_){
        Debug.Log(string.Format(
                    "OnTriggerExit collider_.gameObject: {0}",
                    collider_.gameObject));
        if (collider_.tag == "Player"){
            var target = collider_.gameObject;
            var length = AttackTargetList.Count;
            AttackTargetList.Remove(target);
            var info = string.Format(
                "Remove attack target {0} " + 
                "from AttackTargetList({1} -> {2})",
                target, length, AttackTargetList.Count);
            Debug.Log(info);
        }
    }
}
