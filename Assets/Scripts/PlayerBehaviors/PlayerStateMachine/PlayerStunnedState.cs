using System.Collections;
using UnityEngine;

public class PlayerStunnedState : PlayerBaseState
{
    public override void EnterState(PlayerStateMachine stateMachine)
    {
        stateMachine.StartCoroutine(stateMachine.StunTimer());
    }
    public override void UpdateState(PlayerStateMachine stateMachine)
    {

    }
    public override void ExitState(PlayerStateMachine player)
    {

    }
   
}