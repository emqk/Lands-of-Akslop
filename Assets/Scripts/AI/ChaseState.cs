using UnityEngine;

public class ChaseState : State<EnemyAgent>
{
    public override void EnterState(EnemyAgent owner)
    {
        Debug.Log("Enter chase state");
    }

    public override void ExitState(EnemyAgent owner)
    {
        Debug.Log("Exit chase state");
    }

    public override void UpdateState(EnemyAgent owner)
    {
        Debug.Log("Update chase state");

        if (!owner.blackboard.targetableObject)
        {
            ReturnToIdle(owner);
            return;
        }

        //Cancel chase if target is too far
       /* if (NavMeshUtils.GetDistanceBetweenPointsOnNavMeshPath(owner.GetNavMeshAgent(), owner.transform.position, owner.blackboard.targetableObject.transform.position) > owner.blackboard.sightDistance)
        {
            ReturnToIdle(owner);
            return;
        }
        else
        {
            //Cancel chase if target entered building
            if (owner.blackboard.targetableObject.GetComponent<WorldAgent>())
            {
                if (owner.blackboard.targetableObject.GetComponent<WorldAgent>().destinationObject)
                {
                    if (owner.blackboard.targetableObject.GetComponent<WorldAgent>().destinationObject.GetWorldActorDockedInObject() == owner.blackboard.targetableObject)
                    {
                        ReturnToIdle(owner);
                        return;
                    }
                }
            }
        }*/

        owner.MoveToTargetObject(owner.blackboard.targetableObject);
    }
    void ReturnToIdle(EnemyAgent owner)
    {
        owner.blackboard.targetableObject = null;
        owner.stateMachine.ChangeState(new IdleState());
    }
}
