using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTargetState : State<EnemyAgent>
{
    public override void EnterState(EnemyAgent owner)
    {
        Debug.Log("Enter captureTarget state");
    }

    public override void ExitState(EnemyAgent owner)
    {
        Debug.Log("Exit captureTarget state");
    }

    public override void UpdateState(EnemyAgent owner)
    {
        Debug.Log("Update captureTarget state");

        if (!owner.blackboard.targetableObject)
        {
            ReturnToIdle(owner);
            return;
        }

        if (owner.MoveToTargetObject(owner.blackboard.targetableObject))
        {
            Debug.Log("------------> Yes! return to IdleState");
            ReturnToIdle(owner);
            return;
        }
    }
    void ReturnToIdle(EnemyAgent owner)
    {
        owner.blackboard.targetableObject = null;
        owner.stateMachine.ChangeState(new IdleState());
    }
}
