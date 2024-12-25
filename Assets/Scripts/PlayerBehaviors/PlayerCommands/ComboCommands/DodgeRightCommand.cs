using DG.Tweening;
using UnityEngine;
using System;

public class DodgeRightCommand : ICombatCommand
{

    Animator _animator;
    Transform playerTransform;
    PlayerStateMachine _playerStateMachine;
    public DodgeRightCommand(Animator animator, PlayerStateMachine player, Transform playerTransform)
    {
        _playerStateMachine = player;
        _animator = animator;
        this.playerTransform = playerTransform;
    }
    public static Action OnDodgeBackCombo;
    public void Execute()
    {
        _playerStateMachine.Stamina -= _playerStateMachine.attackStats.StaminaCosts["Dodge"];
        _animator.CrossFade("Dodge Right", 0.1f);
        Vector3 targetPosition = playerTransform.position + playerTransform.right * 3f;
        playerTransform.DOMove(targetPosition, 0.625f);
    }
}
