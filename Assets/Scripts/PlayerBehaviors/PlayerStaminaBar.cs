using UnityEngine;
using UnityEngine.UI;
public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField] Slider staminaSlider;
    PlayerStateMachine playerStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerStateMachine.OnStaminaChange += PlayerStateMachine_OnStaminaChange;
    }

    private void PlayerStateMachine_OnStaminaChange(float staminaRaw)
    {
       
        float staminaNormalized = staminaRaw / (float)playerStateMachine.MaxStamina;
     
        staminaSlider.value = staminaNormalized;
    }

  
}
