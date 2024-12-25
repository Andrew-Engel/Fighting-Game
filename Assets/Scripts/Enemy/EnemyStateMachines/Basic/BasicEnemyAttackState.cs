using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAttackState: BasicEnemyBaseState
{
    List<string> lightAttacks = new List<string>();
    List<string> heavyAttacks = new List<string>();
    List<string> specialAttacks = new List<string>();
    BasicEnemyStateMachine be;

    // failsafe timer to prevent being stuck here for unknown reasons
    public float delay = 4f; // Set the delay time in seconds
    private float timer = 0f;
    private bool isTimerRunning = false;
    public override void EnterState(BasicEnemyStateMachine stateMachine)
    {
        if (lightAttacks.Count == 0) { CreateLists(); }
        if (be == null) {be = stateMachine;}
        PlanAttack(be.staminaBar.Stamina);

        // Start the timer
        isTimerRunning = true;
    }
    public override void UpdateState(BasicEnemyStateMachine stateMachine)
    {

        FacePlayer(stateMachine.playerTransform, stateMachine.modelTransform);

        //timer
        if (isTimerRunning)
        {
            // Increment the timer by the time passed since the last frame
            timer += Time.deltaTime;

            // Check if the timer has reached the delay time
            if (timer >= delay)
            {
                // Stop the timer
                isTimerRunning = false;
                // Call the method
                be.SwitchState(be.fightIdleState);

                
            }
        }

    }
    public override void ExitState(BasicEnemyStateMachine player)
    {

    }
    void PlanAttack(float staminaLevel)
    {
    
        if (be.specialCharged)
        {
            Debug.Log("SpecialAttack");
            int attackIndex = Random.Range(0, specialAttacks.Count - 1);
            be.enemyAnimator.CrossFade(specialAttacks[attackIndex], 0.2f);
        }
        else if (staminaLevel > 20 && staminaLevel < 50)
        {
            be.staminaBar.Stamina -= 10;
            int attackIndex = Random.Range(0, lightAttacks.Count - 1);
            be.enemyAnimator.CrossFade(lightAttacks[attackIndex], 0.2f);
        }
        else if (staminaLevel > 50 && staminaLevel <= 100)
        {
            be.staminaBar.Stamina -= 30;
            int attackIndex = Random.Range(0, heavyAttacks.Count - 1);
            be.enemyAnimator.CrossFade(heavyAttacks[attackIndex], 0.2f);

        }
        else
        {
            be.SwitchState(be.fightIdleState);

        }

    }
    void CreateLists()
    {
       lightAttacks.Add("Jab");
        lightAttacks.Add("Jab+Cross");
        lightAttacks.Add("Push_Kick");

       // heavyAttacks.Add("Jab+Cross+Hook");
        
      

        heavyAttacks.Add("RoundHouse+BackKick");

        specialAttacks.Add("Blitz Attack 1");
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
