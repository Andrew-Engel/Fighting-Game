using UnityEngine;

public class BasicEnemyStunnedState : BasicEnemyBaseState
{
    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
        stateMachine.headAimRig.weight = 0f;
        stateMachine.enemyAnimator.SetBool("Stunned", true);
        stateMachine.StartCoroutine(stateMachine.RecoveryTimer(stateMachine.stunTime));
    }
    public override void UpdateState(BasicEnemyStateMachine stateMachine)
    {
        Debug.Log("No Man's land! Stuned");
    }

    public override void ExitState(BasicEnemyStateMachine stateMachine)
    {
        stateMachine.enemyAnimator.SetBool("Stunned", false);
    }
}
