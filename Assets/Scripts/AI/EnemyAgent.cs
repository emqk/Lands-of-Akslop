using UnityEngine;

public class EnemyAgent : WorldAgent
{
    public StateMachine<EnemyAgent> stateMachine = null;
    public Blackboard blackboard = new Blackboard();

    private void Awake()
    {
        base.Awake();
        stateMachine = new StateMachine<EnemyAgent>(this);
        stateMachine.ChangeState(new IdleState());
    }

    private void Update()
    {
        base.Update();
        stateMachine.Update();
    }

    public void ChaseWorldAgent(WorldAgent agentToChase)
    {
        blackboard.targetableObject = agentToChase;
        stateMachine.ChangeState(new ChaseState());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = MyCountry.GetCountryColor();
        Gizmos.DrawWireSphere(transform.position, blackboard.sightDistance);
    }
}
