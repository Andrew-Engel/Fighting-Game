using DG.Tweening;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerBeatDownState : PlayerBaseState
{
     Animator animator;
     float maxTimeBetweenInputs = 1.5f;
     int maxComboCycles = 60;
    private float lastInputTime;
    private int comboStep = 0;
    private int comboCycle = 0;
    private bool isVulnerable = false;
    float currentTime;
    public bool finisher;

    PlayerStateMachine playerStateMachine;

    IEnemyStateBroadcast enemyState;
    //text counter
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);
    public override void EnterState(PlayerStateMachine stateMachine)
    {

        
        finisher = false;

        stateMachine.beatDownCamera.Priority = 50;
        stateMachine.effectOverHeadLights.SetActive(true);
        stateMachine._audio.PlayOneShot(stateMachine.BeatDownEntrySound);

        playerStateMachine = stateMachine;
        animator = stateMachine._animator;
        stateMachine._animator.SetBool("BeatDown",true);
        lastInputTime = Time.time;

        animator.CrossFade("BeatDown", 0.3f);
        stateMachine._audio.PlayOneShot(stateMachine.BeatDownEntrySound);

        stateMachine.OnAnimationEnd += StateMachine_OnAnimationEnd;
        animator.ResetTrigger("Jab");

        animator.ResetTrigger("Cross");

        EnableHUD();
    }

    private void StateMachine_OnAnimationEnd(object sender, System.EventArgs e)
    {
        Time.timeScale = 1f;
        playerStateMachine.SwitchState(playerStateMachine.attackState);
    }

    public override void UpdateState(PlayerStateMachine stateMachine)
    {
        stateMachine.FaceEnemy();
            if (stateMachine._controls.Player.Attack.WasPressedThisFrame()) // Left click
        {
        

            HandleInput(0);
            }
            else if (stateMachine._controls.Player.HeavyAttack.WasPressedThisFrame() && comboStep != 0) // Right click
            {
     

            HandleInput(1);
            }
        
    }

    void HandleInput(int input)
    {
         currentTime = Time.time;
       // Debug.Log($"Current Time: {currentTime}, Last Input Time: {lastInputTime}, Time Difference: {currentTime - lastInputTime}");

        // Initialize lastInputTime if it's the first input
        if (lastInputTime == 0)
        {
            lastInputTime = currentTime;
        }
        // Check if the input is within the allowed time frame
        if (currentTime - lastInputTime <= maxTimeBetweenInputs)
        { // Check if the input matches the expected combo step
           
            if ((comboStep == 0 && input == 0) || (comboStep == 1 && input == 1))
            {
                PlayAnimation(comboStep);
                comboStep = 1 - comboStep; // Toggle between 0 and 1
                lastInputTime = currentTime;
                comboCycle++;
                AnimateText();
                if (comboCycle >= maxComboCycles)
                {
                   
                    ResetCombo();
                }
            }
            else
            {
           
                ResetCombo();
            }
        }
        else
        {

            ResetCombo();
        }
    }

    void PlayAnimation(int step)
    {
        if (playerStateMachine.TransitCheck())
        {
            playerStateMachine._animator.SetTrigger("Transit");
        }
        else if (playerStateMachine.ShortTransitCheck())
        {
            playerStateMachine._animator.SetTrigger("ShortTransitHigh");
        }
        if (finisher)
        {
            playerStateMachine.finisherCam.Priority = 100;
            Time.timeScale = 0.6f;
            animator.CrossFade("Big Right Slap", 0.3f);
            playerStateMachine.WaitForAnimationToEndExterior("Big Right Slap");
        }
        else
        {

            if (step == 0)
            {
                if (comboCycle == 0)
                    animator.CrossFade("BeatDown_L_1", 0.3f);
                else
                    animator.SetTrigger("BeatDown.L.1");
                playerStateMachine.enemyAnimator.CrossFade("BeatDown_L_1", 0.3f);
            }
            else
            {
                // animator.CrossFade("BeatDown_R_1", 0.3f);
                animator.SetTrigger("BeatDown.R.1");
                playerStateMachine.enemyAnimator.CrossFade("BeatDown_R_1", 0.3f);
            }
        }

    }


    void EnableHUD()
    {
        playerStateMachine.beatDownHUD.DOFade(1f, 1f);
    }
    void DisableHUD()
    {
        playerStateMachine.beatDownHUD.DOFade(0f, 1f);
    }
    public void AnimateText()
    {

        // Kill any existing tweens on the textMeshPro
        playerStateMachine.beatDownCounterText.transform.DOKill();
        playerStateMachine.beatDownCounterText.DOKill();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(playerStateMachine.beatDownCounterText.transform.DOScale(targetScale, playerStateMachine.beatDownCounterAnimationDuration).SetEase(Ease.OutQuad));
        sequence.Join(playerStateMachine.beatDownCounterText.DOColor(playerStateMachine.activeBeatDownCounterColor, playerStateMachine.beatDownCounterAnimationDuration).SetEase(Ease.OutQuad));

        // Add a callback to change the text and play the sound when the text is at its biggest
        sequence.AppendCallback(() =>
        {
            playerStateMachine.beatDownCounterText.text = comboCycle.ToString() + "x";

            playerStateMachine._audio.PlayOneShot(playerStateMachine.beatDownCounterSound,0.5f);
        });

        sequence.AppendInterval(0.5f);
        sequence.Append(playerStateMachine.beatDownCounterText.transform.DOScale(Vector3.one, playerStateMachine.beatDownCounterAnimationDuration).SetEase(Ease.OutQuad));
        sequence.Join(playerStateMachine.beatDownCounterText.DOColor(playerStateMachine.normalBeatDownCounterColor, playerStateMachine.beatDownCounterAnimationDuration).SetEase(Ease.OutQuad));
    }
 

    void ResetCombo()
    {
        comboStep = 0;
        comboCycle = 0;
        lastInputTime = 0;
        playerStateMachine.SwitchState(playerStateMachine.attackState);
    }



    public override void ExitState(PlayerStateMachine player)
    {
        player._audio.PlayOneShot(player.BeatDownExitSound);
        player.effectOverHeadLights.SetActive(false);
        player.beatDownCamera.Priority = 0;
        player.finisherCam.Priority = 0;
        player._animator.SetBool("BeatDown", false);

        player.BeatDownEnd();
        DisableHUD();
    }
}
