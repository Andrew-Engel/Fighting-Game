using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    float timer = 0.0f;
    public bool perfectCounter;
    public bool blocking;
    float drainTimer = 0.0f;
    public override void EnterState(PlayerStateMachine player)
    {
       player._animator.SetBool("Block",true);
        perfectCounter = true;
    }
    public override void UpdateState(PlayerStateMachine player)
    {
        player.FaceEnemy();
        if (perfectCounter) 
        timer += player.perfectCounterWindowCloseRate * Time.deltaTime;
        if (timer > 100.0f)
        {

            perfectCounter=false;
        }
        drainTimer += player.blockingIdleStaminaDrainRate * Time.deltaTime;
        if (timer > 10f)
        {

            player.Stamina--;
        }
    }
    public override void ExitState(PlayerStateMachine player)
    {
        player._animator.SetBool("Block", false);
    }
}
