using DG.Tweening;
using UnityEngine;
using System;

public class IpponSeoiNageCommand : ICombatCommand
{
    Animator _animator;
    PlayerStateMachine _player;

    public IpponSeoiNageCommand(Animator animator, PlayerStateMachine player)
    {
        _player = player;
        _animator = animator;
    }
    public static Action OnIpponSeoiNageCombo;
    public void Execute()
    {
        Debug.Log("ISN");
        if (_player.SP >= 1)
        {
            _player.SP -= 1;
            if (Vector3.Distance(_player.transform.position, _player.nearbyEnemies[_player.enemyFacingIndex].position) < _player.comboRange)
            {
                _player.PlayableDirector.playableAsset = _player.ipponSeoiNageTimeline;

                _player.PlayableDirector.Play();
                if (_player.enemyAnimator == null) _player.AssignEnemyVariables();
                Vector3 targetPosition = _player.playerAttackSceneTransform.position;

                _player.transform.DOMove(targetPosition, 0.625f);
            }
            else
            {
                _player._animator.SetTrigger("ComboMiss");
            }
        }
    }
}
