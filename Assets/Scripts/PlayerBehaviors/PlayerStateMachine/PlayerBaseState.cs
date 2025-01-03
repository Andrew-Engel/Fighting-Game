using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerStateMachine player);
    public abstract void UpdateState(PlayerStateMachine player);
    public abstract void ExitState(PlayerStateMachine player);

}
