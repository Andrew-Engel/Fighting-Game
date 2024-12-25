using System.Windows.Input;
using UnityEngine;

public class HighAttackCommand : ICombatCommand
{
    PlayerStateMachine _playerStateMachine;
    IPlayerCombatAnimations player;
    Animator _animator;
    public HighAttackCommand( Animator animator, IPlayerCombatAnimations player, PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
        _animator = animator;
        this.player = player;
    }
    public void Execute()
    {
        Debug.Log("Jab command execute");
        _playerStateMachine.Stamina -= _playerStateMachine.attackStats.StaminaCosts["JabCommand"];


        if (_playerStateMachine.TransitCheck())
        {
            _animator.SetTrigger("Transit");
        }
        else if (_playerStateMachine.ShortTransitCheck())
        {
            _animator.SetTrigger("ShortTransitHigh");
        }
        else _animator.SetTrigger("Jab");
    }
}
