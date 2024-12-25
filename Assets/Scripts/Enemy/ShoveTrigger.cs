using UnityEngine;

public class ShoveTrigger : MonoBehaviour
{
    IPlayerShoving playerShoving;
    PlayerStateMachine playerStateMachine;
    
    private void Start()
    {
        playerShoving = GetComponentInParent<IPlayerShoving>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerShoving.InIdleState())
            {
                playerStateMachine = collision.gameObject.GetComponent<PlayerStateMachine>();
                playerShoving.ShovePlayer();
            }
        }
    }
}
