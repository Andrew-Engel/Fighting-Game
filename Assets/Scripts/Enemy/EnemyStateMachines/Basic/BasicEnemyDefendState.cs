using UnityEngine;
using DG.Tweening;
public class BasicEnemyDefendState : BasicEnemyBaseState
{
    public Vector3 hitPoint;

    Animator _animator;
    int _counterFrequency;
    BasicEnemyStateMachine _stateMachine;
    string[] middleDefenseAnimationTitles = { "Middle Dodge", "Center High Block" };
    string[] headDefenseAnimationTitles = { "Left High Block", "Center High Block", "Left Pivot" };
    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
        if (_animator == null) { _animator = stateMachine.enemyAnimator; _counterFrequency = stateMachine.counterFrequency; _stateMachine = stateMachine; }
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
        Vector3 effectposition;
        switch (incomingAttack)
        {
            
            case "Middle Hit":

                _animator.CrossFade(PickRandomString(middleDefenseAnimationTitles), 0.2f);
                
                if (hitPoint != null) effectposition = hitPoint;
                else effectposition = _stateMachine.blockEffectLocation.position;
                GameObject.Instantiate(_stateMachine.blockEffect, effectposition, Quaternion.identity);

                // targetPosition = _stateMachine.modelTransform.position  + - 1f *_stateMachine.backDodgeDistance* _stateMachine.modelTransform.forward;
                // _stateMachine.modelTransform.DOMove(targetPosition, _stateMachine.backDodgeTime);
                CounterLottery();
                break;
            case "Head Hit":
                _animator.CrossFade(PickRandomString(headDefenseAnimationTitles), 0.2f);
               
                if (hitPoint != null) effectposition = hitPoint;
                else effectposition = _stateMachine.blockEffectLocation.position;
                GameObject.Instantiate(_stateMachine.blockEffect, effectposition, Quaternion.identity);

                // targetPosition = _stateMachine.modelTransform.position + -1f * _stateMachine.backDodgeDistance * _stateMachine.modelTransform.forward;
                // _stateMachine.modelTransform.DOMove(targetPosition, _stateMachine.backDodgeTime);
                CounterLottery();
                break;
        }
    }
    void CounterLottery()
    {
        if (Random.Range(0, _counterFrequency) <= 1)
        {
            _stateMachine.SwitchState(_stateMachine.counterAttackState);
        }
        else
        {
            _stateMachine.SwitchState(_stateMachine.fightIdleState);
        }
    }
    string PickRandomString(string[] input)
    {
        int index = Random.Range(0, input.Length);
        return input[index];
    }
}
