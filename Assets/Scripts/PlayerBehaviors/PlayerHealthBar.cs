using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    PlayerStateMachine playerStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerStateMachine.OnHealthChange += PlayerStateMachine_OnHealthChange;
    }

    private void PlayerStateMachine_OnHealthChange(int healthRaw)
    {
       float healthNormalized = healthRaw / playerStateMachine.MaxHP;
        healthSlider.value = healthNormalized;
    }

    
}
