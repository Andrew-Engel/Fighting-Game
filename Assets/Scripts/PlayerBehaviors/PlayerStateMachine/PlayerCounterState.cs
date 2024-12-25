using UnityEngine;
using DG.Tweening;

public class PlayerCounterState : PlayerBaseState
{
    public string incomingAttack;
    public Animator enemyAnimator;
    public IEnemyCounterDetection enemy;
    public Transform enemyTransform;
    PlayerControls playerControls;
    PlayerStateMachine _player;
    private bool isTimeSlowed = false;
    private float slowTimeDuration = 1.5f;
    private float slowTimeStart;
    public Vector3 counterEffectLocation;

    //UI
    CanvasGroup counterIndicatorCG;
    public override void EnterState(PlayerStateMachine player)
    {
        playerControls = player._controls;
        _player = player;
        if (counterIndicatorCG == null) {counterIndicatorCG = GameObject.Find("CounterIndicator").GetComponent<CanvasGroup>();}
      
        if (enemy != null) 
        { Debug.Log("Making enemy vulnerbale");
            enemy.CounteredByPlayer(); }
        SlowDownTime();
        player.SP++;

        GameObject.Instantiate(player.counterEffect, counterEffectLocation, Quaternion.identity);
        player._audio.PlayOneShot(player.counterSound);
      

    }
    public override void UpdateState(PlayerStateMachine player)
    {
      


        if (isTimeSlowed)
        {
            //Debug.Log("SlowMoTimer: "+ (Time.unscaledTime - slowTimeStart));
            if (Time.unscaledTime - slowTimeStart >= slowTimeDuration)
            {
                ResetTimeScale();
            }
            else
            {
                CheckInput();
            }
        }

        player.FaceEnemy();
    }
    public override void ExitState(PlayerStateMachine player)
    {
        Time.timeScale = 1f;
        player._animator.SetBool("Counter", false);
        counterIndicatorCG.DOFade(0f, 0.8f);
    }
    void ResetTimeScale()
    {
        enemy.RecoverFromCounter(); 
     
        isTimeSlowed = false;
        _player.SwitchState(_player.attackState);
    }
    void SlowDownTime()
    {
        counterIndicatorCG.DOFade(1f, 0.8f);
        isTimeSlowed = true;
        Time.timeScale = 0.5f;
        slowTimeStart = Time.unscaledTime;
    }
    void CheckInput()
    {
       
        if (playerControls.Player.Attack.WasPressedThisFrame())
        {
            
           
            RunCounter();
        }
    }
    void RunCounter()
    {
        Debug.Log("RunningCounter!");
       
        _player.SwitchState(_player.beatDownState);

        /*
        switch (incomingAttack)
        {
            case "Middle.M":
                _player.transform.DOMove(enemyTransform.position, 0.7f);
                //player._animator.CrossFade("Side Kick to Hook Kick Counter", 0.2f);
                _player._animator.SetTrigger("Middle_Middle_Counter");

                break;
            case "Middle.L":
                _player.transform.DOMove(enemyTransform.position, 0.7f);
                _player._animator.SetTrigger("Middle_Left_Counter");

                break;
            case "Middle.R":
                _player.transform.DOMove(enemyTransform.position, 0.7f);
                // player._animator.CrossFade("Side Kick to Hook Kick Counter", 0.2f);
                _player._animator.SetTrigger("Middle_Right_Counter");

                break;

        }*/
    }
   
}
