using Cinemachine;
using UnityEngine;

public class PlayerHitReaction : MonoBehaviour
{
    [SerializeField] AudioClip[] smallHitSounds, bigHitSounds;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] float smallHitSoundVolume,bigHitVolume, smallHitCollisionForce, bigHitCollisionForce;
   [SerializeField] CinemachineImpulseSource _impulseSource;
    PlayerStateMachine _statemachine;
    [SerializeField] int smallDamageEffectCutoff = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _statemachine = GetComponent<PlayerStateMachine>();
        if ( _statemachine != null ) 
        _statemachine.OnGettingHit += _statemachine_OnGettingHit;
    }
    
    private void _statemachine_OnGettingHit(int damage)
    {
       // Debug.Log("Damage value: "+ damage);
        if (damage<=smallDamageEffectCutoff)
        {
            _impulseSource.GenerateImpulse(smallHitCollisionForce);
            int i = Random.Range(0, smallHitSounds.Length);
            _audioSource.PlayOneShot(smallHitSounds[i], smallHitSoundVolume);
        }
        else
        {
            _impulseSource.GenerateImpulse(bigHitCollisionForce);
            int i = Random.Range(0, bigHitSounds.Length);
            _audioSource.PlayOneShot(bigHitSounds[i], bigHitVolume);
        }
        

    }

  
}
