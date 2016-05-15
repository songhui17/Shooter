using UnityEngine;
using System.Collections.Generic;

public class IdleBehavior : StateMachineBehaviour {

    public float _duration = 2;
    public List<string> TriggerList = new List<string>();

    public List<string> SetOnEnter = new List<string>();
    public List<string> ResetOnEnter = new List<string>();

    public List<string> SetOnExit = new List<string>();
    public List<string> ResetOnExit = new List<string>();

    [SerializeField]
    private float _enterTime;
    [SerializeField]
    private float _currentTime;

    private bool _inState = false;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
        if (currentState.fullPathHash == stateInfo.fullPathHash) return;

        _enterTime = Time.realtimeSinceStartup;
        // Debug.Log(string.Format("OnStateEnter: {0}, _enterTime: {1}", animator, _enterTime));
        _inState = true;
        foreach (var prop in SetOnEnter) {
            animator.SetBool(prop, true);
        }

        foreach (var prop in ResetOnEnter) {
            animator.SetBool(prop, false);
        }
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_inState) return;

        var count = TriggerList.Count;
        if (count == 0) return;

        _currentTime = Time.realtimeSinceStartup;
        if (_currentTime - _enterTime > _duration){
            foreach (var prop in SetOnExit) {
                animator.SetBool(prop, true);
            }

            foreach (var prop in ResetOnExit) {
                animator.SetBool(prop, false);
            }

            var random = (int)Random.Range(0, count);
            animator.SetTrigger(TriggerList[random]);
            // Debug.Log("OnStateUpdate: " + TriggerList[random]);

            _inState = false;
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
