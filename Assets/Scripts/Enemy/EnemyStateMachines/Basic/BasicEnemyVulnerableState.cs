using UnityEngine;

public class BasicEnemyVulnerableState : BasicEnemyBaseState
{
    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
        stateMachine.enemyAnimator.SetBool("Vulnerable",true);
        stateMachine._ai.enabled = false;

        
    }
    public override void UpdateState(BasicEnemyStateMachine stateMachine)
    {
        FacePlayer(stateMachine.playerTransform, stateMachine.modelTransform);
    }
    public override void ExitState(BasicEnemyStateMachine stateMachine)
    {
        stateMachine.enemyAnimator.SetBool("Vulnerable", false);
        stateMachine._ai.enabled = true;
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
