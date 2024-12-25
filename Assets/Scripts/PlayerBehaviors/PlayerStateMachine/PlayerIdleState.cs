using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateMachine stateMachine)
    {
        stateMachine.headAimRig.weight = 0f;
        stateMachine._animator.SetBool("FightIdle", false);
      //  stateMachine.thirdPersonController.LockCameraPosition = false;
    }
    public override void UpdateState(PlayerStateMachine stateMachine)
    {
        if (stateMachine._controls.Player.Attack.WasPerformedThisFrame() || stateMachine._controls.Player.HeavyAttack.WasPerformedThisFrame())
        {
            if (stateMachine.nearbyEnemies.Count > 0) 
            stateMachine.SwitchState(stateMachine.fightIdleState);
            else
                stateMachine.FindEnemy();
        }
    }
    public override void ExitState(PlayerStateMachine player)
    {

    }
}