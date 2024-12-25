using System;
using UnityEngine;

public class JabCrossCommand: ICombatCommand
{
 
    Animator _animator;
    PlayerStateMachine _playerStateMachine;
    public JabCrossCommand(Animator animator, PlayerStateMachine player)
    {
        _playerStateMachine = player;
         _animator = animator;
    }
    public static Action OnJabCrossCombo;
    public void Execute()
    {
        Debug.Log("Jabcross command execute");
        _playerStateMachine.Stamina -= _playerStateMachine.attackStats.StaminaCosts["JabCrossCommand"];
        _animator.CrossFade("Jab+Cross", 0.1f);
    }
    
}
