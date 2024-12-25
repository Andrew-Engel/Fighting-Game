using UnityEngine;

public interface IEnemyStateBroadcast
{
    public BaseState CurrentState();
    public bool InAttackState();
    public bool IsNearDeath();
    public bool InVulnerableState();
}
