using UnityEngine;

public interface IEnemyCounterDetection
{
    void KnockedDownByPlayer();
    void CounteredByPlayer();
    void RecoverFromCounter();
    Animator ReturnEnemyAnimator();
    Transform GetEnemyTransform();
}
