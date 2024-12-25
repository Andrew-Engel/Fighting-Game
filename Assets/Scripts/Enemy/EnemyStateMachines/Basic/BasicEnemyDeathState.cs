using UnityEngine;

public class BasicEnemyDeathState: BasicEnemyBaseState
{
    public override void EnterState(BasicEnemyStateMachine be)
    {
     be.puppetMaster.state = RootMotion.Dynamics.PuppetMaster.State.Dead;
        Debug.Log("Death state");
    }
    public override void UpdateState(BasicEnemyStateMachine be)
    {
       
    }
    public override void ExitState(BasicEnemyStateMachine be)
    {
       
    }
}
