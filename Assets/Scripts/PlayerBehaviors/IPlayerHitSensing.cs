using UnityEngine;

public interface IPlayerHitSensing
{
    void HitReaction(string reactionAnimation, int rawDamage, Collision collision);
    void HitReaction(string incomingAttack, int rawDamage, Vector3 contactPoint, IEnemyCounterDetection enemyCounterDetection);
    bool EnemyAttackStateCheck();
}
