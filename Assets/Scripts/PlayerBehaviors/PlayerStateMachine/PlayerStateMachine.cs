using UnityEngine;
using UnityEngine.InputSystem;
using Tutorial.PatternCombo;
using NUnit.Framework;
using System.Collections.Generic;
using StarterAssets;
using RootMotion.Dynamics;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using System.Collections;
using Cinemachine;
using System;
using UnityEngine.Animations.Rigging;
using System.Threading;
using TMPro;
[RequireComponent(typeof(ThirdPersonController))]
[RequireComponent(typeof(CinemachineImpulseSource))]
public class PlayerStateMachine : MonoBehaviour, IPlayerHitSensing, IPlayerCombatAnimations
{
    //states
    public PlayerBaseState currentState;
    public PlayerIdleState idleState = new PlayerIdleState();
    public PlayerFightIdleState fightIdleState = new PlayerFightIdleState();
    public PlayerAttackState attackState = new PlayerAttackState();
    public PlayerBeatDownState beatDownState = new PlayerBeatDownState();
    public PlayerBlockState blockState = new PlayerBlockState();
    public PlayerStunnedState stunnedState = new PlayerStunnedState();
    public PlayerDeathState deathState = new PlayerDeathState();
    public PlayerCounterState counterState = new PlayerCounterState();

   public Animator _animator;
    
    ComboActionQueueManager comboActionQueueManager;

   public PlayerControls _controls;
     Vector2 look;
    public int attackTarget = 1;
    //Stats
    public int lightDamage, heavyDamage;
    public float perfectCounterWindowCloseRate = 2f;
    public event Action<int> OnSpecialCharge;
    public event EventHandler OnInadequateSpecialPoints;
    private int _sp = 0;
    public int SP
    {
        get {
            if (_sp == 0)
            {
                OnInadequateSpecialPoints?.Invoke(this, new EventArgs());
            }
            
            return _sp; }
        set
        {
            _sp = value;
            if (OnSpecialCharge != null)
            {
                OnSpecialCharge(value);
            }
           
        }
    }
    public float staminaRegenerationRate = 1f;
    public event Action<int> OnHealthChange;
    private int _hp;
    public int HP
    {
        get { return _hp; }
        set { _hp = value; 
        if (OnHealthChange != null)
            {
                OnHealthChange(value);
            }
        if (_hp <= 0)
            {
                Debug.Log("PlayerDeath");
            }
        
        }
    }
    public int MaxHP;

    public event Action<float> OnStaminaChange;
    private float _stamina;
    public float Stamina
    {
        get { return _stamina; }
        set
        {
            _stamina = value;
            if (OnStaminaChange != null)
            {
                OnStaminaChange(value);
            }
            if (_stamina <= 0)
            {
                _stamina = 0;
                Debug.Log("Player Exhaustion");
            }

        }
    }
    public int MaxStamina;
   

    public float blockingIdleStaminaDrainRate = 1f;
    public PlayerAttackStats attackStats;
    //In game cutscenes
    public PlayableDirector PlayableDirector;
   public TimelineAsset chainPunchTimeline,finalComboTimeline, ipponSeoiNageTimeline;
    public Transform playerAttackSceneTransform;
    public Animator enemyAnimator;
    public float comboRange;
    //Third person controller
    public ThirdPersonController thirdPersonController;
    //Player facing Enemies
    public List<Transform> nearbyEnemies = new List<Transform>();
    bool enemiesNearby;
    public int enemyFacingIndex = -1;
    public float enemyDetectionDistance;
    public LayerMask enemySightTargetLayer,enemyDamageLayer;
    //enemy variables for state checks
    public IEnemyStateBroadcast enemyState;
    //look variables
    //Cameras
   
    public float lockOnDampingFactor,cameraLockOnDampingFactor;
    public Transform playerFollowCamAimTarget;
    public Vector3 highAttackLookOffset, middleAttackLookOffset, lowAttackLookOffset;
    public float jumpEnemyDisengageDistance = 2f;
    bool checkForLanding;
    [Tooltip("This is the closest a player can get to a locked on enemy before forward movement towards the enemy is disabled")]
    [SerializeField] float minimumDistanceToEnemy;
    // Animation Rigging
    public Rig headAimRig;
    public Transform headAimTarget;
    public Vector3 aimTargetOffset = new Vector3(0f,0f,0f);
    //effects
    CinemachineImpulseSource impulseSource;
    public GameObject effectOverHeadLights;
    public AudioClip BeatDownEntrySound,BeatDownExitSound;
    public AudioSource _audio;
    public ParticleSystem blockEffect, hitEffect, bloodyHitEffect;

    //Animation polishing
    [SerializeField] float transitionAnimationThreshold, shortTransitionAnimationThreshold, homingAnimationTime;
    // player attackpoints
    public Transform rightFootTransform, leftFootTransform, rightHandTransform, leftHandTransform;
    //combo system
   public AttackInvoker _attackInvoker;
    // attack reactions
    [SerializeField] float shoveStumbleDistance, shoveStumbleTime,stunTime;
    public event Action<int> OnGettingHit;
    public float armorFactor;

    //State specific variables
    // Counter
    public ParticleSystem counterEffect;
   
    public AudioClip counterSound;
    //beatdown
    public Color normalBeatDownCounterColor, activeBeatDownCounterColor;
    public float beatDownCounterAnimationDuration;
    public AudioClip beatDownCounterSound;
    public CanvasGroup beatDownHUD;
    public TextMeshProUGUI beatDownCounterText;
   public bool beatDownReady = true;
    [SerializeField] float beatDownCoolDownTime = 5f;
    public event EventHandler OnBeatDownEnd;
    public CinemachineVirtualCamera beatDownCamera,finisherCam;
    private void Awake()
    {
        _controls = new PlayerControls();
        _controls.Player.Block.started += ctx => EnterBlock();
        _controls.Player.Block.canceled += ctx => ExitBlock();

    }
  
 
    private void OnEnable()
    {
        _controls.Enable();
    }
    private void OnDisable()
    {
        _controls.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        attackStats = GetComponent<PlayerAttackStats>();
       
        thirdPersonController = GetComponent<ThirdPersonController>();
        //setup invoker and combo manager
        comboActionQueueManager = GetComponent<ComboActionQueueManager>();
        _attackInvoker = new AttackInvoker(comboActionQueueManager);
        //state initialization
       currentState = idleState;
        currentState.EnterState(this);
        HP = MaxHP;
        Stamina = MaxStamina;
        impulseSource= GetComponent<CinemachineImpulseSource>();
    }
    float staminaTimer;
    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
        CheckForDodgeInputs();

        if (checkForLanding && thirdPersonController.Grounded)
        {
            SwitchState(fightIdleState);
            checkForLanding = false;
        }

        if (_stamina < MaxStamina)
        {
            staminaTimer -= Time.deltaTime;
            if (staminaTimer <= 0f) {
            
                Stamina += staminaRegenerationRate * Time.deltaTime;
            }
        }
        if (currentState == attackState )
        {
            if (FacingEnemyVulnerable())
            {
              //  StartCoroutine(BeatDownCoolDown());
                Debug.Log("Switching to beatdown state");

               enemyAnimator = nearbyEnemies[enemyFacingIndex].GetComponentInParent<IEnemyCounterDetection>().ReturnEnemyAnimator();
                SwitchState(beatDownState);

            }
        }
       else if (currentState == beatDownState)
        {
            if (enemyState == null)
            {
                enemyState = nearbyEnemies[enemyFacingIndex].GetComponentInParent<IEnemyStateBroadcast>();
            }
                if (enemyState.IsNearDeath()) beatDownState.finisher = true;
            if (!FacingEnemyVulnerable())
            {
                Debug.Log("Enemy noyt vulnerable, switching off of beatdown state");
                SwitchState(attackState);

            }
        }

    }
    //checks for animation end, then triggers event that should be accessible by discrete states to use logic
    public event EventHandler OnAnimationEnd;
    // available method for non monobehavior classes (the states)
    public void WaitForAnimationToEndExterior(string stateName)
    {
        StartCoroutine(WaitForAnimationToEnd(stateName));
    }
    // Waiting for animation to end to trigger logic
    System.Collections.IEnumerator WaitForAnimationToEnd(string stateName)
    {
        // Wait until the specified animation state is playing
        while (!this._animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        // Wait until the animation state has finished playing
        while (this._animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // Trigger the method after the animation has completed
        OnAnimationEnd?.Invoke(this, EventArgs.Empty);
    }
    //State transits
    public void SwitchState(PlayerBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;

        state.EnterState(this);
    }

    void EnterBlock()
    {
        SwitchState(blockState);
    }
    void ExitBlock()
    {
        SwitchState(fightIdleState);
    }
    private IEnumerator BeatDownCoolDown()
    {
        beatDownReady = false;
        yield return new WaitForSecondsRealtime(beatDownCoolDownTime);
        beatDownReady = true;
    }
    //Transit between states
    public void BeatDownEnd()
    {
        Debug.Log("BeatDownEND()");
        OnBeatDownEnd?.Invoke(this, EventArgs.Empty);
    }
    // Find and face enemies
    public void FindEnemy()
    {
        if (nearbyEnemies.Count > 0) { nearbyEnemies.Clear(); }
        Collider[] enemyColliders = Physics.OverlapSphere(this.transform.position, this.enemyDetectionDistance, this.enemySightTargetLayer, QueryTriggerInteraction.Collide);

        foreach (Collider collider in enemyColliders)
        {
            nearbyEnemies.Add(collider.gameObject.GetComponent<Transform>());
        }
        if (nearbyEnemies.Count > 0) enemiesNearby = true;
        else enemiesNearby = false;
    }
    public bool CheckEnemyTooClose()
    {
        float distance = Vector3.Distance(this.transform.position, nearbyEnemies[enemyFacingIndex].transform.position);
       
        if (distance <= minimumDistanceToEnemy) return true;
        else return false;

    }
    public void FindClosestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < nearbyEnemies.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, nearbyEnemies[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        if (closestIndex != enemyFacingIndex)
        {
            enemyFacingIndex = closestIndex;
            UpdateEnemyState();
        }
    }
    void UpdateEnemyState()
    {
        if (enemyFacingIndex >= 0 && enemyFacingIndex < nearbyEnemies.Count)
        {
            enemyState = nearbyEnemies[enemyFacingIndex].GetComponentInParent<IEnemyStateBroadcast>();
        }
        else
        {
            enemyState = null;
        }
    }
    // Check for vulnerable state to enable beatdown mechanic
    bool FacingEnemyVulnerable()
    {
        if (enemyState == null)
        {
            enemyState = nearbyEnemies[enemyFacingIndex].GetComponentInParent<IEnemyStateBroadcast>();
       
            
        }
        if (enemyState.InVulnerableState()) return true;
        else return false;
    }
   // checks constantly for dodge commands( so that they can be included in the command queue?) 
    void CheckForDodgeInputs()
    {
        if (currentState != idleState)
        {
            if (_controls.Player.DodgeBack.WasCompletedThisFrame())
            {
                
                ICombatCommand combatCommand = new BackInputCommand();
                _attackInvoker.AddCommand(combatCommand);
            }
            if (_controls.Player.DodgeLeft.WasCompletedThisFrame())
            {
             
                ICombatCommand combatCommand = new LeftInputCommand();
                _attackInvoker.AddCommand(combatCommand);
            }
            if (_controls.Player.DodgeRight.WasCompletedThisFrame())
            {

                ICombatCommand combatCommand = new RightInputCommand();
                _attackInvoker.AddCommand(combatCommand);
            }
        }
    }
    // Spacing
    //Player spacing methods to ensure player is facing chosen enemy and in correct distance
    public void FaceEnemy()
    {
        if (enemiesNearby)
        {
            Transform target = nearbyEnemies[enemyFacingIndex].transform;
            Vector3 targetVector = new Vector3(target.position.x, this.transform.position.y, target.position.z);
            // stateMachine.transform.LookAt(targetVector);
            Vector3 camAimTargetVector = targetVector + aimTargetOffset;
            headAimTarget.position = camAimTargetVector;
            var rotation = Quaternion.LookRotation(targetVector - this.transform.position);
            var cameraRotation = Quaternion.LookRotation(camAimTargetVector - this.playerFollowCamAimTarget.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * this.lockOnDampingFactor);
            playerFollowCamAimTarget.rotation = Quaternion.Slerp(this.playerFollowCamAimTarget.rotation, cameraRotation, Time.deltaTime * this.cameraLockOnDampingFactor);
            CheckForAboveEnemy(target);

            //align head aim
            headAimTarget.position = camAimTargetVector;
        }

    }
    void CheckForAboveEnemy(Transform enemyTransform)
    {
        if (Vector3.Distance(this.transform.position, enemyTransform.position) < jumpEnemyDisengageDistance && !thirdPersonController.Grounded)
        {
            SwitchState(idleState);
            checkForLanding = true;
        }
      
    }
   public bool TransitCheck()
    {
        if (Vector3.Distance(this.transform.position, playerAttackSceneTransform.position) > transitionAnimationThreshold)
        {
            return true;
        }
        else return false;
    }
 public bool ShortTransitCheck()
    { float distance = Vector3.Distance(this.transform.position, playerAttackSceneTransform.position);
        if (distance > shortTransitionAnimationThreshold && distance < transitionAnimationThreshold)
        {
            return true;
        }
        else return false;
    }
   
    // Hit reactions
    IEnemyCounterDetection enemyCounterDetection;

    public void HitReaction(string incomingAttack, int rawDamage, Collision collision)
    {
        if (currentState == blockState)
        {
            Instantiate(blockEffect, collision.GetContact(0).point, Quaternion.identity);
           
            switch (incomingAttack)
            {
                case "Middle.M":
                    _animator.SetTrigger("Middle_Middle_Block");
                    
                    break;
                case "Middle.L":
                    _animator.SetTrigger("Middle_Left_Block");

                    break;
                case "Middle.R":
                    _animator.SetTrigger("Middle_Right_Block");

                    break;
                case "Head":

                    _animator.SetTrigger("Head_Middle_Block");

                    break;

            }
            
            if (blockState.perfectCounter)
            {
                 enemyCounterDetection = collision.gameObject.GetComponentInParent<IEnemyCounterDetection>();
                if (enemyCounterDetection != null)
                {
                    counterState.enemy=enemyCounterDetection;
                    counterState.incomingAttack = incomingAttack;
                    enemyAnimator = enemyCounterDetection.ReturnEnemyAnimator();
                    counterState.enemyAnimator = enemyAnimator;
                  
                        playerAttackSceneTransform = enemyCounterDetection.GetEnemyTransform();
                    counterState.enemyTransform = playerAttackSceneTransform;
                }
                _animator.SetBool("Counter",true);
                counterState.counterEffectLocation = collision.GetContact(0).point;
                SwitchState(counterState);
            }
        }
        else if (currentState != counterState)
        {
            impulseSource.GenerateImpulse();
            Debug.Log("Player is hit! NOT blocked!");
            int damage = (int)(rawDamage * armorFactor);
            if (OnGettingHit != null)
            OnGettingHit(damage);
            HP -= damage;
            if (incomingAttack == "Head") Instantiate(bloodyHitEffect, collision.GetContact(0).point, Quaternion.identity);
            else
            Instantiate(hitEffect, collision.GetContact(0).point, Quaternion.identity);
            switch (incomingAttack)
            {
                
                case "Middle.M":
                    _animator.CrossFade("Middle Hit_M", 0.25f);
                    break;
                case "Middle.L":
                    _animator.CrossFade("Middle Hit_L", 0.25f);
                    break;
                case "Middle.R":
                    _animator.CrossFade("Middle Hit_R", 0.25f);
                    break;
                case "Head":
                  
                    int index = UnityEngine.Random.Range(0, 2);
                    _animator.CrossFade("Head Hit" + " " + index, 0.25f);
                    break;

            }
            SwitchState(stunnedState);
        }
    }
    //different overload without collisions needed
    public void HitReaction(string incomingAttack, int rawDamage, Vector3 contactPoint, IEnemyCounterDetection enemyCounterDetection)
    {
        if (currentState == blockState)
        {
            Instantiate(blockEffect, contactPoint, Quaternion.identity);

            switch (incomingAttack)
            {
                case "Middle.M":
                    _animator.SetTrigger("Middle_Middle_Block");

                    break;
                case "Middle.L":
                    _animator.SetTrigger("Middle_Left_Block");

                    break;
                case "Middle.R":
                    _animator.SetTrigger("Middle_Right_Block");

                    break;
                case "Head":

                    _animator.SetTrigger("Head_Middle_Block");

                    break;

            }

            if (blockState.perfectCounter)
            {
              
                if (enemyCounterDetection != null)
                {
                    counterState.enemy = enemyCounterDetection;
                    counterState.incomingAttack = incomingAttack;
                    enemyAnimator = enemyCounterDetection.ReturnEnemyAnimator();
                    counterState.enemyAnimator = enemyAnimator;

                    playerAttackSceneTransform = enemyCounterDetection.GetEnemyTransform();
                    counterState.enemyTransform = playerAttackSceneTransform;
                }
                _animator.SetBool("Counter", true);
                counterState.counterEffectLocation = contactPoint;
                SwitchState(counterState);
            }
        }
        else if (currentState != counterState)
        {
            Debug.Log("Player is hit! NOT blocked!");
            int damage = (int)(rawDamage * armorFactor);
            if (OnGettingHit != null)
                OnGettingHit(damage);
            HP -= damage;
            if (incomingAttack == "Head") Instantiate(bloodyHitEffect, contactPoint, Quaternion.identity);
            else
            Instantiate(hitEffect, contactPoint, Quaternion.identity);
            switch (incomingAttack)
            {

                case "Middle.M":
                    _animator.CrossFade("Middle Hit_M", 0.25f);
                    break;
                case "Middle.L":
                    _animator.CrossFade("Middle Hit_L", 0.25f);
                    break;
                case "Middle.R":
                    _animator.CrossFade("Middle Hit_R", 0.25f);
                    break;
                case "Head":

                    int index = UnityEngine.Random.Range(0, 2);
                    _animator.CrossFade("Head Hit" + " " + index, 0.25f);
                    break;

            }
            SwitchState(stunnedState);
        }
    }
    //publicly accessible methods for enemy classes to access
    public void KnockDownEnemy()
    {
        if (enemyCounterDetection != null) { enemyCounterDetection.KnockedDownByPlayer(); }
    }
    public void AssignEnemyVariables()
    {
        if (enemyAnimator == null)
        {
            enemyCounterDetection = nearbyEnemies[enemyFacingIndex].gameObject.GetComponentInParent<IEnemyCounterDetection>();
            enemyAnimator = enemyCounterDetection.ReturnEnemyAnimator();
            playerAttackSceneTransform = enemyCounterDetection.GetEnemyTransform();
        }
    }
    public void ReceiveShove()
    {
        OnGettingHit(0);
        SwitchState(stunnedState);
        _animator.CrossFade("ShoveReaction", 0.2f);
        this.transform.DOMove(this.transform.forward * -1 * shoveStumbleDistance + this.transform.position, shoveStumbleTime);
    }


    public void ToggleMovement()
    {
       
        if (thirdPersonController.movementEnabled)
        {
            thirdPersonController.movementEnabled = false;
        }
        else
        {
            thirdPersonController.movementEnabled = true;
        }
    }

    public void ChainPunchEnd()
    {
        KnockDownEnemy();
    }
    public void FinalComboEnd()
    {
        KnockDownEnemy();
    }
   
    public IEnumerator StunTimer()
    {
        thirdPersonController.movementEnabled = false;
        yield return new WaitForSeconds(stunTime);
        SwitchState(fightIdleState);
        thirdPersonController.movementEnabled = true;
    }

    public bool EnemyAttackStateCheck()
    {
        IEnemyStateBroadcast enemy = nearbyEnemies[enemyFacingIndex].GetComponentInParent<IEnemyStateBroadcast>();
        if (enemy.InAttackState() )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
