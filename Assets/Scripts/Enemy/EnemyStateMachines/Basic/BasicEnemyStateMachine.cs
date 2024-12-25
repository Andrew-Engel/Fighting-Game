using Cinemachine;
using RootMotion.Dynamics;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

public class BasicEnemyStateMachine : MonoBehaviour, IEnemyHitSensing, IEnemyCounterDetection, IPlayerShoving, IEnemyStateBroadcast
{
    //States
    public BasicEnemyBaseState currentState;
    public BasicEnemyAttackState attackState = new BasicEnemyAttackState();
    public BasicEnemyDefendState defendState = new BasicEnemyDefendState();
    public BasicEnemyFightIdleState fightIdleState = new BasicEnemyFightIdleState();
    public BasicEnemyIdleState idleState = new BasicEnemyIdleState();
    public BasicEnemyKnockDownState knockDownState = new BasicEnemyKnockDownState();
    public BasicEnemyStunnedState stunnedState = new BasicEnemyStunnedState();
    public BasicEnemyVulnerableState vulnerableState = new BasicEnemyVulnerableState();
    public BasicEnemyCounterAttackState counterAttackState = new BasicEnemyCounterAttackState();
    public BasicEnemyDeathState deathState = new BasicEnemyDeathState();
    //Variables
    public PuppetMaster puppetMaster;
    public NavMeshAgent _ai;
    public string incomingAttack;
    public float strikingDistance = 1f;
    public Transform playerTransform;
    public Transform modelTransform;
    [SerializeField] Transform playerCounterTransform;
    public ParticleSystem blockEffect;
    public Transform blockEffectLocation;
    public PlayerStateMachine _playerStateMachine;
    //Difficulty stats
    public int counterFrequency;
    public int defenseFrequency;
    public int knockDownFrequency;
    public float knockDownLength;
    public float stunTime;
    public float enemyAttackCoolDown;
    //attack stats
    public int lightAttackDamage, heavyAttackDamage;
    //Defend State
    public float backDodgeDistance, backDodgeTime;
    public Transform incomingPlayerLimb;
    //VulnerableState
    public bool isNearDeath;
    //Charging for super combo
    public int ComboChargeCount
    {
        get {return _comboChargeCount;}
        set
        {
            _comboChargeCount = value;
            if (_comboChargeCount > specialComboThreshold)
            {
                PowerUp();
            }
        }
    }
    int _comboChargeCount = 0;
    public int specialComboThreshold = 3;
    public GameObject normalCloak, glowingCloak;
    public GameObject[] powerUpEffects;
    public ParticleSystem powerUpParticle;
    public bool specialCharged = false;
    //shoving
    public float jumpBackDistance = 0.5f;
    //HP
    EnemyHealthBar healthBar;
    public UnityEvent<int> OnDamage;

    public float damageResistanceFactor = 1f;
    //Stamina
    public EnemyStaminaBar staminaBar;
    //Animations
    public Animator enemyAnimator;
    //effects
    CinemachineImpulseSource _impulseSource;
    public Transform headAimTarget;
    public Vector3 headAimOffset;
   public Rig headAimRig;
    public ParticleSystem hitEffect, bloodyHitEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        currentState = fightIdleState;
        currentState.EnterState(this);


        //Assigning variabls
        healthBar = GetComponent<EnemyHealthBar>();
        healthBar.OnDeath += HealthBar_OnDeath;
        healthBar.OnNearDeath += HealthBar_OnNearDeath;
        staminaBar = GetComponent<EnemyStaminaBar>();
        staminaBar.OnExhaust += StaminaBar_OnExhaust;
        staminaBar.OnRecover += StaminaBar_OnRecover;



        _playerStateMachine = playerTransform.GetComponent<PlayerStateMachine>();
        _playerStateMachine.OnBeatDownEnd += _playerStateMachine_OnBeatDownEnd;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void HealthBar_OnNearDeath(object sender, EventArgs e)
    {
     isNearDeath = true;
    }

    //event methods
    private void _playerStateMachine_OnBeatDownEnd(object sender, EventArgs e)
    {
        Debug.Log("Player beatdown ended, getting out of vulnerbale state");
      if (currentState == vulnerableState) SwitchState(fightIdleState);
    }

    private void StaminaBar_OnRecover(object sender, EventArgs e)
    {
        if (currentState != deathState)
            SwitchState(fightIdleState);
    }

    private void StaminaBar_OnExhaust(object sender, System.EventArgs e)
    {
      //  if (currentState != deathState)
       // SwitchState(vulnerableState);

    }

    private void HealthBar_OnDeath(object sender, System.EventArgs e)
    {
        SwitchState(deathState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
        Debug.Log("EnemyCurrentState: "+ currentState.ToString());
        enemyAnimator.SetFloat("Speed", _ai.velocity.magnitude);

        if (Vector3.Distance(this.modelTransform.position, playerTransform.position) <= jumpBackDistance && currentState != attackState)
        {
            enemyAnimator.CrossFade("Dodge_Bwd", 0.3f);
        }
        AimHead();
    }
    public void HitReaction(string animation,float impulse, Transform incomingLimb, Vector3 hitPoint)
    {
       
        if (_playerStateMachine.currentState != _playerStateMachine.idleState && _playerStateMachine.currentState != _playerStateMachine.fightIdleState)
        {
              incomingPlayerLimb = incomingLimb;

            if (RunDefenseLottery())
            {
                Debug.Log("running defense");
                incomingAttack = animation;
                defendState.hitPoint = hitPoint;
                SwitchState(defendState);
            }
            else
            {
                
                    staminaBar.Stamina -= 5;
                    _impulseSource.GenerateImpulse();
                    if (animation == "Head Hit") Instantiate(bloodyHitEffect, hitPoint, Quaternion.identity);
                    else
                    Instantiate(hitEffect, hitPoint, Quaternion.identity);
                    switch (animation)
                    {
                        case "Low Hit":

                            break;
                        case "Middle Hit":
                            enemyAnimator.CrossFade(animation, 0.25f);
                            break;
                        case "Head Hit":
                            int index = UnityEngine.Random.Range(0, 2);
                            enemyAnimator.CrossFade(animation + " " + index, 0.25f);
                            break;

                    }

                    int adjustedDamage = (int)(impulse * damageResistanceFactor);
                    healthBar.Health -=adjustedDamage;
                    Debug.Log("damaging enemy by:"+ adjustedDamage);
                        OnDamage.Invoke(adjustedDamage);
                    KnockDownLottery();
                
            }
        }

    }
    // Overload to receive hit without colision input (e.g. from animation)
    public void HitReaction(string animation, int rawDamage, Vector3 hitPoint, Transform incomingLimb)
    {

        
            incomingPlayerLimb = incomingLimb;
          if (RunDefenseLottery())
            {
                Debug.Log("running defense");
                incomingAttack = animation;
            defendState.hitPoint = hitPoint;
            SwitchState(defendState);
            }
       
          else
        {
          
                staminaBar.Stamina -= 5;
                _impulseSource.GenerateImpulse();
                if (animation == "Head Hit") Instantiate(bloodyHitEffect, hitPoint, Quaternion.identity);
                else
                    Instantiate(hitEffect, hitPoint, Quaternion.identity);
                switch (animation)
                {
                    case "Low Hit":

                        break;
                    case "Middle Hit":
                        enemyAnimator.CrossFade(animation, 0.25f);
                        break;
                    case "Head Hit":
                        int index = UnityEngine.Random.Range(0, 2);
                        enemyAnimator.CrossFade(animation + " " + index, 0.25f);
                        break;

                }

                int adjustedDamage = (int)(rawDamage * damageResistanceFactor);
                healthBar.Health -= adjustedDamage;
                
                OnDamage.Invoke(adjustedDamage);
                if (currentState != vulnerableState)
                    KnockDownLottery();
            
        }
        

    }
    public bool AbleToReceiveHits()
    {
        if (currentState == knockDownState) return false;
        else return true;
    }



    public void SwitchState(BasicEnemyBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;

        state.EnterState(this);
    }
    bool RunDefenseLottery()
    {
        if (currentState != vulnerableState)
        {
            float staminaFactor = staminaBar.Stamina / staminaBar.maxStamina;
            if (staminaFactor < 0.1f) { staminaFactor = 0.1f; }
            float oddsFloat = defenseFrequency * staminaFactor;
            int oddsInt = (int)oddsFloat;
            if (UnityEngine.Random.Range(0, oddsInt) <= 1)
            {
                return true;
            }
            else { return false; }
        }
        else { return false; }
        
    
    }
    void KnockDownLottery()
    {
        float staminaFactor = staminaBar.Stamina / staminaBar.maxStamina;
        if (staminaFactor < 0.1f) { staminaFactor = 0.1f; }
        float oddsFloat = knockDownFrequency / staminaFactor;
        int oddsInt = (int)oddsFloat;

        if (oddsInt < 1) oddsInt = 1;
        if (UnityEngine.Random.Range(0, oddsInt) == 1)
        {
            SwitchState(knockDownState);
        }
    }
    public IEnumerator RecoveryTimer(float length)
    {
        yield return new WaitForSeconds(length);
         if (currentState == stunnedState)
        {
            SwitchState(fightIdleState);
        }



    }
    public IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(enemyAttackCoolDown);
        fightIdleState.readyToAttack = true;
    }

    public void KnockedDownByPlayer()
    {
        SwitchState(knockDownState);
    }
  public void CounteredByPlayer()
    {
        SwitchState(vulnerableState);
    }
    public void RecoverFromCounter()
    {
        SwitchState(fightIdleState);
    }
    public void ShovePlayer()
    {
        enemyAnimator.CrossFade("Shove", 0.2f);
        _playerStateMachine.ReceiveShove();
    }

    void PowerUp()
    {
        glowingCloak.SetActive(true);
        normalCloak.SetActive(false);
        _impulseSource.GenerateImpulseWithForce(1.5f);
        Instantiate (powerUpParticle,this.transform.position,Quaternion.identity);
        foreach (GameObject g in powerUpEffects) { g.SetActive(true); }
    }
  
    // send current state to player to affect player behaviors such as beatdown mode
    public Animator ReturnEnemyAnimator()
    {

        return enemyAnimator;

    }
    public Transform GetEnemyTransform()
    {
        return this.playerCounterTransform;
    }
    public BaseState CurrentState()
    {
        return currentState;
    }
    public bool IsNearDeath()
    {
        return isNearDeath;
    }
    public bool InVulnerableState()
    {
        if (currentState == vulnerableState)
        {
            return true;
        }
        else return false;
    }
        public bool InAttackState()
    {
        if (currentState == attackState || currentState == counterAttackState)
        {
            return true;
        }
        else return false;
    }
    public bool InIdleState()
    {
        if (currentState == idleState || currentState == fightIdleState) { return true; }
        else return false;
    }
    // Align rig aim target to player for more realistic head tracking to player
    void AimHead()
    {
        headAimTarget.position = playerTransform.position + headAimOffset;
    }
}
