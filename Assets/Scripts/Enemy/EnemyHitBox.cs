using UnityEngine;
using UnityEngine.VFX;

public class EnemyHitBox : MonoBehaviour
{
    IEnemyHitSensing enemyHitSensing;
    [SerializeField] string reactionAnimationName;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyHitSensing = GetComponentInParent<IEnemyHitSensing>();
    }
    private void OnCollisionEnter(Collision collision)
    {
    
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.relativeVelocity.magnitude >1f && enemyHitSensing.AbleToReceiveHits())
        {
           // Instantiate(effect,collision.GetContact(0).point,Quaternion.identity);
            enemyHitSensing.HitReaction(reactionAnimationName, collision.impulse.magnitude, collision.transform, collision.GetContact(0).point);
        }
    }
    public void DamageEnemyExterior(int damage, Transform incomingPlayerLimb)
    {
        enemyHitSensing.HitReaction(reactionAnimationName, damage, this.transform.position, incomingPlayerLimb);
      
    }
}
