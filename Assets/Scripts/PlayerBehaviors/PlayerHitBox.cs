using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    IPlayerHitSensing playerHitSensing;
    [SerializeField] string reactionAnimationName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHitSensing = GetComponentInParent<IPlayerHitSensing>();
    }
    private void OnCollisionEnter(Collision collision)
    {
      

        if (collision.gameObject.layer == 9 && collision.relativeVelocity.magnitude > 10f )
        {
            Debug.Log("player hit initial");
            if (playerHitSensing.EnemyAttackStateCheck())

            {
                float damage = collision.impulse.magnitude;
                int damageInt = (int)damage;
                playerHitSensing.HitReaction(reactionAnimationName, damageInt, collision);
            }


            
        }
    }
    //methods for outside classes to run damage logic
    public void DamagePlayerExterior(int damage, IEnemyCounterDetection counterDetection)
    {
        playerHitSensing.HitReaction(reactionAnimationName, damage, this.transform.position, counterDetection);
    }
}
