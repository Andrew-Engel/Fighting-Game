using UnityEngine;

public class BasicEnemyKnockDownState : BasicEnemyBaseState
{
    float timer = 0.0f;
    BasicEnemyStateMachine _be;
    public bool ragDollEnemy = true;
    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
     if (_be == null) _be = stateMachine;
     if (ragDollEnemy)
        stateMachine.puppetMaster.state = RootMotion.Dynamics.PuppetMaster.State.Dead;
        //stateMachine.StartCoroutine(stateMachine.RecoveryTimer(stateMachine.knockDownLength));
    }
    public override void UpdateState(BasicEnemyStateMachine stateMachine)
    {
        Debug.Log("Knowckdown state");
        timer +=( 100 * Time.deltaTime);
        if (timer > stateMachine.knockDownLength)
        {
            GetUp();
           stateMachine.SwitchState(stateMachine.fightIdleState);
        }
    }
    public override void ExitState(BasicEnemyStateMachine stateMachine)
    {
        ragDollEnemy = true;
    }
    public void GetUp()
    {
        timer = 0.0f;
        _be.puppetMaster.state = RootMotion.Dynamics.PuppetMaster.State.Alive;
     
    }
}
