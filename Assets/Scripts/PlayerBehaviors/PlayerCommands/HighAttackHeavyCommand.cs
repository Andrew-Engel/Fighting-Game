using UnityEngine;

public class HighAttackHeavyCommand: ICombatCommand
{
    
    Animator _animator;
    PlayerStateMachine _player;
    public HighAttackHeavyCommand( Animator animator, PlayerStateMachine player)
    {

     _player = player;
        _animator = animator;
    }
    public void Execute()
    {
        _player.Stamina -= _player.attackStats.StaminaCosts["Cross"];
        

        if (_player.TransitCheck())
        {
            _animator.SetTrigger("Transit");
        }
        else if (_player.ShortTransitCheck())
        {
            _animator.SetTrigger("ShortTransitHigh");
        }
        else _animator.SetTrigger("Cross");
    }
}
