using UnityEngine;

public class BasicEnemyFightIdleState : BasicEnemyBaseState
{
    bool homing;
    public bool readyToAttack = true;
    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
        stateMachine.headAimRig.weight = 1f;
    }
    public override void UpdateState(BasicEnemyStateMachine stateMachine)
    {
        FacePlayer(stateMachine.playerTransform, stateMachine.modelTransform);
        if (homing && stateMachine._ai.isActiveAndEnabled)
        {
            stateMachine._ai.SetDestination(stateMachine.playerTransform.position);
        }
        if (stateMachine._ai.isActiveAndEnabled && homing && Vector3.Distance(stateMachine.playerTransform.position, stateMachine.modelTransform.position) < stateMachine.strikingDistance)
        {

            stateMachine._ai.SetDestination(stateMachine.modelTransform.position);
            homing = false;
            if (readyToAttack)
            {
                stateMachine.StartCoroutine(stateMachine.AttackCoolDown());

                stateMachine.SwitchState(stateMachine.attackState);
                readyToAttack = false;
            }
        }
        else if (!homing) { homing = true; }
        else { Debug.Log("No Man's land! Fight Idle"); }
    }
    public override void ExitState(BasicEnemyStateMachine player)
    {

    }
    void FacePlayer(Transform playerTransform, Transform enemyTransform)
    {
        // Calculate direction to the player
        Vector3 direction = (playerTransform.position - enemyTransform.position).normalized;

        // Create a rotation based on the direction
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Rotate the agent to face the player
        enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, lookRotation, Time.deltaTime * 5f);
    }

}
