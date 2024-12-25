using UnityEngine;
using System;
using DG.Tweening;

public class DodgeLeftCommand : ICombatCommand
{

    Animator _animator;
    Transform playerTransform;
    PlayerStateMachine _playerStateMachine;
    public DodgeLeftCommand(Animator animator, PlayerStateMachine player, Transform playerTransform)
    {
        _playerStateMachine = player;
        _animator = animator;
        this.playerTransform = playerTransform;
    }
    public static Action OnDodgeBackCombo;
    public void Execute()
    {
        _playerStateMachine.Stamina -= _playerStateMachine.attackStats.StaminaCosts["Dodge"];
        _animator.CrossFade("Dodge Left", 0.1f);
        Vector3 targetPosition = playerTransform.position + playerTransform.right * -1f * 3f;
        playerTransform.DOMove(targetPosition, 0.625f);
    }
}
