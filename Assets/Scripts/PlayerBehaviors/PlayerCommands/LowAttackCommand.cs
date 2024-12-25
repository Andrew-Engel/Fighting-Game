using UnityEngine;

public class LowAttackCommand : ICombatCommand
{
   
    Animator _animator;
   public LowAttackCommand( Animator animator)
    {

       _animator = animator;
    }
    public void Execute()
    {
        _animator.CrossFade("Low Kick", 0.1f);
    }
}
