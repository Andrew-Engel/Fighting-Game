using UnityEngine;

public class MiddleAttackCommand : ICombatCommand
{
  
    Animator _animator;
    public MiddleAttackCommand( Animator animator)
    {

      
        _animator = animator;
    }
    public void Execute()
       {
        _animator.CrossFade("Middle Kick", 0.1f);
    }
}
