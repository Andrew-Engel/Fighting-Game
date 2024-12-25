using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public override void EnterState(PlayerStateMachine stateMachine)
    {
        stateMachine.aimTargetOffset = stateMachine.highAttackLookOffset;

    }
    public override void UpdateState(PlayerStateMachine stateMachine)
    {
        stateMachine.FaceEnemy();
      

        if (stateMachine._controls.Player.Attack.WasPerformedThisFrame() && !stateMachine._controls.Player.RaiseTarget.IsPressed() && !stateMachine._controls.Player.LowerTarget.IsPressed())
        {
            ICombatCommand combatCommand = new HighAttackCommand(stateMachine._animator, stateMachine, stateMachine);
            stateMachine._attackInvoker.AddCommand(combatCommand);

        }
        else if (stateMachine._controls.Player.HeavyAttack.WasPerformedThisFrame() && !stateMachine._controls.Player.RaiseTarget.IsPressed() && !stateMachine._controls.Player.LowerTarget.IsPressed())
        {
            ICombatCommand combatCommand = new HighAttackHeavyCommand(stateMachine._animator, stateMachine);
            stateMachine._attackInvoker.AddCommand(combatCommand);
        }
        else if  (stateMachine._controls.Player.Attack.WasPerformedThisFrame() && stateMachine._controls.Player.RaiseTarget.IsPressed())
            {
                ICombatCommand combatCommand = new MiddleAttackCommand(stateMachine._animator);
                stateMachine._attackInvoker.AddCommand(combatCommand);
            }
        else if (stateMachine._controls.Player.Attack.WasPerformedThisFrame() && stateMachine._controls.Player.LowerTarget.IsPressed())
        {
            ICombatCommand combatCommand = new LowAttackCommand(stateMachine._animator);
            stateMachine._attackInvoker.AddCommand(combatCommand);
        }
        //switch to nearest enemy
        if (stateMachine._controls.Player.SwitchEnemy.WasPerformedThisFrame())
        {
            stateMachine.FindClosestEnemy();
        }
        
    }
    public override void ExitState(PlayerStateMachine player)
    {

    }
}
