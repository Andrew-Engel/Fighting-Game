using DG.Tweening;
using UnityEngine;

public class BasicEnemyBlockState: BasicEnemyBaseState
{//this is no longer used as of 12 21 24
    Animator _animator;
    BasicEnemyStateMachine _stateMachine;

    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
        if (_animator == null) { _animator = stateMachine.enemyAnimator; _stateMachine = stateMachine; }
        Defend(stateMachine.incomingAttack);
    }
    public override void UpdateState(BasicEnemyStateMachine stateMachine)
    {

    }
    public override void ExitState(BasicEnemyStateMachine stateMachine)
    {

    }
    void Defend(string incomingAttack)
    {
        switch (incomingAttack)
        {
            case "Middle Hit":
                _animator.CrossFade("Middle Block", 0.2f);
                _stateMachine.SwitchState(_stateMachine.fightIdleState);
                GameObject.Instantiate(_stateMachine.blockEffect, _stateMachine.blockEffectLocation.position, Quaternion.identity);
                break;
        }
    }


}
