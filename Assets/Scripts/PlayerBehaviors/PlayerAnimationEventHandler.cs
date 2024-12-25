using Cinemachine;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using System.Collections;
using static RootMotion.Demos.FKOffset;
using System.Linq;

public class PlayerAnimationEventHandler : MonoBehaviour
{
   
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] float explosionPower = 2f;
    // hit source locations
    [SerializeField] Transform rightFootTransform;
    [SerializeField] Transform leftFootTransform;
    [SerializeField] Transform leftHandTransform;
    [SerializeField] Transform rightHandTransform;
    //
    [SerializeField] CinemachineImpulseSource impulseSource;
    ThirdPersonController thirdPersonController;
    PlayerStateMachine playerStateMachine;
    public AudioClip[] attackSounds,hitSounds,bigHitSounds, stepSounds, droppedSounds;
    AudioSource _audioSource;
    [SerializeField] float stepVolume, whooshVolume, droppedSoundVolume, hitVolume,bigHitVolume;
    public float moveForwardDistance, moveForwardTime, moveForwardTimeLongRange, smallbackwardDistance;

    public float moveToEnemyOffset;
    //counter effects
    [SerializeField] CinemachineVirtualCamera counterCam;
    [SerializeField] float slowMoFactor = 0.5f;
    public AudioClip slowMoSound, undoSlowMoSound;

    //attack variables
    [Tooltip("Distance for detection of closest player hitbox so that enemy attacks do not need to be so precise")]
    [SerializeField] float hitBoxDetectionRadius;
    private EnemyHitBox closestHitBox;
    [SerializeField] LayerMask enemyHitBoxLayer;
    private void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        _audioSource = GetComponent<AudioSource>();
    }
  //movements
    public void MoveForwardSlow()
    {
        this.transform.DOMove(playerStateMachine.playerAttackSceneTransform.position, moveForwardTimeLongRange);
    }
    public void MoveForwardFast()
    {
        this.transform.DOMove(playerStateMachine.playerAttackSceneTransform.position, moveForwardTime);
    }
    public void MoveBackward()
    {
        this.transform.DOMove(this.transform.position += this.transform.forward * moveForwardDistance * -1f, moveForwardTime);
    }
    public void MoveBackwardSmall()
    {
        this.transform.DOMove(this.transform.position += this.transform.forward * smallbackwardDistance, 0.5f);
    }
    public void DisableMovement()
    {
        thirdPersonController.movementEnabled = false;
    }
    public void EnableMovement()
    {
        thirdPersonController.movementEnabled = true;
    }
    public void MoveToEnemy()
    {
        Vector3 target = playerStateMachine.nearbyEnemies[playerStateMachine.enemyFacingIndex].position;
        Vector3 direction = target - this.transform.position;
        // Step 2: Normalize the direction vector
        direction.Normalize();
        // Step 3: Calculate the current distance between the player and the target
        float currentDistance = Vector3.Distance(this.transform.position, target);

        // Step 4: Calculate the distance the player needs to move to be X units away from the target
        float moveDistance = currentDistance - moveToEnemyOffset;

        // Ensure the move distance is not negative
        moveDistance = Mathf.Max(moveDistance, 0);

        // Step 5: Multiply the unit vector by the calculated distance
        Vector3 offset = direction * moveDistance;

        // Step 6: Add the offset to the player's position
        Vector3 pointPosition = this.transform.position + offset;
        // Step 7: Ensure the movement is restricted to the XZ plane
        pointPosition.y = this.transform.position.y;
        this.transform.DOMove(pointPosition, moveForwardTime);
    }
    // methods for switching states
    public void ExitBeatDown()
    {
        Debug.Log("Exit Beatdown");
        playerStateMachine.SwitchState(playerStateMachine.attackState);
    }
    //attack effects
            //Knockdowns
    public void RightFootKnockDown()
    {

        
        Explosion(rightFootTransform.position);
    }
    public void LeftFootKnockDown()
    {
        Explosion(leftFootTransform.position);
    }
    public void RightHandKnockDown()
    {


        Explosion(rightHandTransform.position);
    }
    public void LeftHandKnockDown()
    {
        Explosion(leftHandTransform.position);
    }
    void Explosion(Vector3 explosionPos)
    {
        impulseSource.GenerateImpulse();
        // Applies an explosion force to all nearby rigidbodies
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)

            {
                rb.AddExplosionForce(explosionPower, explosionPos, explosionRadius, 3.0F);
                Debug.Log("coutner explosion");
            }
        }
    }
             //Attack methods to ensure damage of enemy
    public void LeftHandAttack()
    {
        EnemyHitBox hitbox = FindClosestPlayerHitBox(leftHandTransform);

        if (hitbox != null)
        {
            hitbox.DamageEnemyExterior(playerStateMachine.lightDamage,leftHandTransform);
        }
    }
    public void RightHandAttack()
    {
        EnemyHitBox hitbox = FindClosestPlayerHitBox(rightHandTransform);

        if (hitbox != null)
        {
            hitbox.DamageEnemyExterior(playerStateMachine.lightDamage, rightHandTransform);
        }
    }
    public void RightFootAttack()
    {
        EnemyHitBox hitbox = FindClosestPlayerHitBox(rightFootTransform);

        if (hitbox != null)
        {
            hitbox.DamageEnemyExterior(playerStateMachine.heavyDamage, rightFootTransform);
        }
    }
    public void LeftFootAttack()
    {
        EnemyHitBox hitbox = FindClosestPlayerHitBox(leftFootTransform);

        if (hitbox != null)
        {
            hitbox.DamageEnemyExterior(playerStateMachine.heavyDamage, leftFootTransform);
        }
    }
    public void FinishEnemy()
    {
        EnemyHitBox hitbox = FindClosestPlayerHitBox(leftHandTransform);
        if (hitbox != null)
        {
            hitbox.DamageEnemyExterior(playerStateMachine.heavyDamage*=2, leftHandTransform);
        }
    }
    EnemyHitBox FindClosestPlayerHitBox(Transform source)
    {
       
        Collider[] hitColliders = Physics.OverlapSphere(source.position, hitBoxDetectionRadius, enemyHitBoxLayer);
        Debug.Log("amount of hit colliders on enemy: +" + hitColliders.Length.ToString());
        float closestDistance = hitBoxDetectionRadius;
        closestHitBox = null;

        foreach (Collider collider in hitColliders)
        {
            EnemyHitBox hitBox = collider.GetComponent<EnemyHitBox>();
            if (hitBox != null)
            {
             
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

    //Sound Effects
    public void PlayStepSound()
    {
        int i = Random.Range(0, stepSounds.Length);
        _audioSource.PlayOneShot(stepSounds[i], stepVolume);
    }
    public void PlayHitSound()
    {
        int i = Random.Range(0, hitSounds.Length);
        _audioSource.PlayOneShot(hitSounds[i], hitVolume);
        impulseSource.GenerateImpulse();
    }
    public void PlayBigHitSound()
    {
        int i = Random.Range(0, bigHitSounds.Length);
        _audioSource.PlayOneShot(bigHitSounds[i], bigHitVolume);
        impulseSource.GenerateImpulse();
    }
    public void PlayAttackSound()
    {
        int i = Random.Range(0, attackSounds.Length);
        _audioSource.PlayOneShot(attackSounds[i], whooshVolume);
    }
    //Attack Reactions
    public void PlayerDropped()
    {
        if (impulseSource != null)
        impulseSource.GenerateImpulse();
        int i = Random.Range(0, droppedSounds.Length);
        _audioSource.PlayOneShot(droppedSounds[i], droppedSoundVolume);
    }
    //Counter effects
    public void SwitchToCounterCam()
    {
        counterCam.Priority = 35;
      
    }
    public void DisengageCounterCam()
    {
        Debug.Log("DisengageCounterCam");
        counterCam.Priority = 0;
    }
    public void EngageSlowMo()
    {
    Time.timeScale = slowMoFactor;
        _audioSource.PlayOneShot(slowMoSound,0.5f);
        StartCoroutine(TimeResumeCountDown());
    }
    public void DisEngageSlowMo()
    {
        Time.timeScale = 1f;
        _audioSource.PlayOneShot(undoSlowMoSound);
    }
    private IEnumerator TimeResumeCountDown()
    {
        yield return new WaitForSecondsRealtime(5);
        if (Time.timeScale != 1f)
        {
            DisEngageSlowMo();
            DisengageCounterCam();
        }
    }
}
