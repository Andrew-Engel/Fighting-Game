using UnityEngine;

public class BasicEnemyCounterAttackState: BasicEnemyBaseState
{
    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
        stateMachine.ComboChargeCount++;
     
        stateMachine.enemyAnimator.SetTrigger("Counter");
        stateMachine.SwitchState(stateMachine.fightIdleState);
    }
    public override void UpdateState(BasicEnemyStateMachine stateMachine)
    {

    }
    public override void ExitState(BasicEnemyStateMachine player)
    {

    }
}
