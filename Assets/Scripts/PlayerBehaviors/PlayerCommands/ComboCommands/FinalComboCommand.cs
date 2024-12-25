using DG.Tweening;
using UnityEngine;

public class FinalComboCommand
{
    Animator _animator;
    PlayerStateMachine _player;

    public FinalComboCommand(Animator animator, PlayerStateMachine player)
    {
        _player = player;
        _animator = animator;
    }
    public void Execute()
    {
        if (_player.SP >= 4)
        {
            _player.SP -= 4;
            if (Vector3.Distance(_player.transform.position, _player.nearbyEnemies[_player.enemyFacingIndex].position) < _player.comboRange)
            {
                _player.PlayableDirector.playableAsset = _player.finalComboTimeline;

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
