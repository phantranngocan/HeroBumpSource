using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOutEvent : StateMachineBehaviour
{
    private bool actionExecuted = false;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float normalizedTime = stateInfo.normalizedTime;
        if (normalizedTime >= 0.2f && !actionExecuted)
        {
            actionExecuted = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actionExecuted = false;
    }
}
