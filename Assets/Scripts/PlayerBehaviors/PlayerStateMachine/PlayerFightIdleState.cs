using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerFightIdleState : PlayerBaseState
{
    Transform enemyTransform;
  
    Transform playerTransform;
    
    public override void EnterState(PlayerStateMachine stateMachine)
    {
        stateMachine.headAimRig.weight = 1f;
        playerTransform = stateMachine.transform;
        stateMachine.FindEnemy();
        stateMachine._animator.SetBool("FightIdle", true);
     //   stateMachine.thirdPersonController.LockCameraPosition = true;
    }
    public override void UpdateState(PlayerStateMachine stateMachine)
    {
        stateMachine.FaceEnemy();
        stateMachine.FindClosestEnemy();
        if (stateMachine._controls.Player.Attack.WasPerformedThisFrame() || stateMachine._controls.Player.HeavyAttack.WasPerformedThisFrame())
        {
            stateMachine.SwitchState(stateMachine.attackState);

        }
        if (stateMachine._controls.Player.Disengage.WasPerformedThisFrame())
        {
            stateMachine.SwitchState(stateMachine.idleState);
        }
     
    }
    public override void ExitState(PlayerStateMachine player)
    {

    }
 
}