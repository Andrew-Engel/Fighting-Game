using DG.Tweening;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BasicEnemyAnimationEventHandler : MonoBehaviour
{
    public float forwardMovementDistance = 2f;
 
    public float backwardMovementDistance = 2f;
    public float backwardMovementTime = 0.8f;
    [SerializeField] float pivotHorizontalDistance, pivotLongitudinalDistance;
    //enemy classes
    PuppetMaster puppet;
    BasicEnemyStateMachine basicEnemyStateMachine;
   [SerializeField] NavMeshAgent navMeshAgent;
    //Sound effects
    public AudioClip[] attackSounds, stepSounds;
    AudioSource _audioSource;
    [SerializeField] float stepVolume, whooshVolume;
    // Getting Thrown
    public Transform throwRecipientPosition;
    //Player info
    Transform playerTransform, enemyPivotLocation;
    //attack variables
    [Tooltip("Distance for detection of closest player hitbox so that enemy attacks do not need to be so precise")]
    [SerializeField] float detectionRadius;
    private PlayerHitBox closestHitBox;
    [SerializeField] Transform leftFoot,rightFoot,leftHand,rightHand;
    [SerializeField] LayerMask playerLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        basicEnemyStateMachine = GetComponentInParent<BasicEnemyStateMachine>();
        puppet = GetComponentInParent<PuppetMaster>();
        _audioSource = GetComponent<AudioSource>();

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        enemyPivotLocation = GameObject.Find("EnemyPivotLocation").GetComponent<Transform>();

        throwRecipientPosition = GameObject.Find("EnemyCounterLocationThrows").GetComponent<Transform>();
    }
    //Movements
   public void AttackEnd()
    {
      if (basicEnemyStateMachine.currentState == basicEnemyStateMachine.attackState)
        basicEnemyStateMachine.SwitchState(basicEnemyStateMachine.fightIdleState);
    }
    public void MoveForward()
    {
        // this.transform.DOMove(this.transform.forward * forwardMovementDistance + this.transform.position, forwardMovementTime);
        //navMeshAgent.SetDestination(this.transform.forward * forwardMovementDistance + this.transform.position);
        if (playerTransform != null)
        {
            // Calculate direction to the target
            Vector3 direction = (playerTransform.position - this.transform.position).normalized;

            // Calculate the new position
            Vector3 newPosition = this.transform.position + direction * forwardMovementDistance;
           if (navMeshAgent != null && navMeshAgent.isOnNavMesh) 
            navMeshAgent.SetDestination(newPosition);
        }
        else { navMeshAgent.SetDestination(this.transform.forward * forwardMovementDistance + this.transform.position); }
        
    }
    public void MoveBackwards()
    {
        // Step 1: Calculate the target position by moving backwards along the local Z-axis
        Vector3 targetPosition = this.transform.localPosition + Vector3.back * backwardMovementDistance;
        this.transform.DOLocalMove(targetPosition, backwardMovementTime);
    }
    public void MoveLeft()
    {
        this.transform.DOMove(-1f * backwardMovementDistance * this.transform.right + this.transform.position, backwardMovementTime);
    }
    public void MoveTowardsPlayer()
    {

    }
    public void LeftPivot()
    {
        Debug.Log("Pivoting");
       
        navMeshAgent.SetDestination(enemyPivotLocation.position);
    }
    // Attack reactions
    public void RagDollEnable(float recoveryTime)
    {
        puppet.state = PuppetMaster.State.Dead;
        StartCoroutine(TimedRecovery(recoveryTime));
    }
    IEnumerator TimedRecovery(float time)
    {

     yield return new WaitForSecondsRealtime(time);
        puppet.state = PuppetMaster.State.Alive;
    
    }
    public void GetInThrowReceptionPosition()
    {
        basicEnemyStateMachine.knockDownState.ragDollEnemy = false;
        basicEnemyStateMachine.SwitchState(basicEnemyStateMachine.knockDownState);
        if (throwRecipientPosition != null)
        this.transform.DOMove(throwRecipientPosition.position, 0.75f);
    }
    public void FlingForward()
    {
        this.transform.DOMove(this.transform.forward * forwardMovementDistance + this.transform.position, 1f);
    }
    //Audio
    public void PlayStepSound()
    {
        int i = Random.Range(0, stepSounds.Length);
        _audioSource.PlayOneShot(stepSounds[i], stepVolume);
    }
    public void PlayAttackSound()
    {
        int i = Random.Range(0, attackSounds.Length);
        _audioSource.PlayOneShot(attackSounds[i], whooshVolume);
    }
    //Attacks
    public void KnockDownPlayer()
    {
        bool playerInRange = Physics.CheckSphere(this.transform.position, detectionRadius*1.5f, playerLayer);
        if (playerInRange)
        basicEnemyStateMachine.ShovePlayer();
    }
    // attacks
    public void LeftFootAttack()
    {

        PlayerHitBox hitbox = FindClosestPlayerHitBox(leftFoot);

        if ( hitbox != null)
        {
            hitbox.DamagePlayerExterior(basicEnemyStateMachine.heavyAttackDamage, basicEnemyStateMachine);
        }
       
    }
    public void RightFootAttack()
    {
        PlayerHitBox hitbox = FindClosestPlayerHitBox(rightFoot);

        if (hitbox != null)
        {
            //calls a method inside the referenced hitbox to deliver damage to simulate a direct hit
            hitbox.DamagePlayerExterior(basicEnemyStateMachine.heavyAttackDamage, basicEnemyStateMachine);
        }
    }
    public void LeftHandAttack()
    {
        PlayerHitBox hitbox = FindClosestPlayerHitBox(leftHand);

        if (hitbox != null)
        {
            hitbox.DamagePlayerExterior(basicEnemyStateMachine.heavyAttackDamage, basicEnemyStateMachine);
        }
    }
    public void RightHandAttack()
    {
        PlayerHitBox hitbox = FindClosestPlayerHitBox(rightHand);

        if (hitbox != null)
        {
            hitbox.DamagePlayerExterior(basicEnemyStateMachine.heavyAttackDamage, basicEnemyStateMachine);
        }
    }

    void Attack(Transform sourceLocation, int damage)
    {
        FindClosestPlayerHitBox(sourceLocation).DamagePlayerExterior(damage, basicEnemyStateMachine);
    }
    PlayerHitBox FindClosestPlayerHitBox(Transform source)
    {
        Collider[] hitColliders = Physics.OverlapSphere(source.position, detectionRadius, playerLayer);
      
        float closestDistance = detectionRadius;
        closestHitBox = null;

        foreach (Collider collider in hitColliders)
        {
            PlayerHitBox hitBox = collider.GetComponent<PlayerHitBox>();
            if (hitBox != null)
            {
                Debug.Log("Player hitbox registered");
                float distance = Vector3.Distance(source.position, hitBox.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHitBox = hitBox;
                }
            }
        }
 
        return closestHitBox;
    }

}
