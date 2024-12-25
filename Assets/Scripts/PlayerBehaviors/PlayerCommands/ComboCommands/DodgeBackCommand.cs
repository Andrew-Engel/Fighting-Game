using UnityEngine;
using System;
using DG.Tweening;
public class DodgeBackCommand : ICombatCommand
{

    Animator _animator;
    Transform playerTransform;
    PlayerStateMachine _playerStateMachine;
    public DodgeBackCommand(Animator animator, PlayerStateMachine player, Transform playerTransform)
    {
        _playerStateMachine = player;
        _animator = animator;
        this.playerTransform = playerTransform;
    }
   
    public void Execute()
    {
        _playerStateMachine.Stamina -= _playerStateMachine.attackStats.StaminaCosts["Dodge"];
        _animator.CrossFade("Jab+Cross", 0.1f);
        _animator.CrossFade("Dodge Backward", 0.1f);
        Vector3 targetPosition =  playerTransform.position + playerTransform.forward * -1f * 3f;
        playerTransform.DOMove(targetPosition, 0.625f);
    }

}
