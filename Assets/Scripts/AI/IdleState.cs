using UnityEngine;

public class IdleState : State<EnemyAgent>
{
    public override void EnterState(EnemyAgent owner)
    {
        Debug.Log("Enter idle state");
    }

    public override void ExitState(EnemyAgent owner)
    {
        Debug.Log("Exit idle state");
    }

    public override void UpdateState(EnemyAgent owner)
    {
        Debug.Log("Update idle state", owner);

        GoToMyNearestCity(owner);
    }
    void GoToMyNearestCity(EnemyAgent owner)
    {
        TargetableObject building = null;
        TargetableObject dockedIn = owner.GetDockedIn();

        if (dockedIn)
        {
            if (dockedIn.GetComponent<City>())
            {
                building = dockedIn;
                dockedIn.GetArmy().MoveAllUnitsToOtherArmy(owner.GetArmy());
               // dockedIn.GetArmy().MoveUnitToOtherArmy(0, owner.GetArmy());
                //GoToMyPosition(owner);
            }
            else
            {
                building = owner.MyCountry.GetNearestUndockedCity(owner);
                owner.MoveToTargetObject(owner);
            }
        }
        else
        {
            building = owner.MyCountry.GetNearestUndockedCity(owner);
            if (building)
            {
                owner.MoveToTargetObject(building);
            }
            else
            {
                GoToMyPosition(owner);
            }
        }
    }
    void GoToMyPosition(EnemyAgent owner)
    {
        owner.MoveToPosition(owner.transform.position);
        Debug.Log("No building im going to my pos");
    }
}