using UnityEngine;

public abstract class BasicEnemyBaseState: BaseState
{
    public abstract void EnterState(BasicEnemyStateMachine be);
    public abstract void UpdateState(BasicEnemyStateMachine be);
    public abstract void ExitState(BasicEnemyStateMachine be);
}
