using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStaminaBar : MonoBehaviour
{
    public float staminaRegenRate = 1;
    float timer;
    //Lerp timer
    public Slider staminaBar;
    public int maxStamina = 100;
    public int exhaustThreshold = 20;
    bool exhausted;
    [SerializeField] private int _stamina;
    public int Stamina
    {

        get { return _stamina; }
        set
        {
          
            _stamina = value;
            if (_stamina < 0) _stamina = 0;
            staminaBar.value = _stamina;
            if (OnStaminaChange != null)
            { OnStaminaChange(value); }
            if (value <= exhaustThreshold)
            {
                exhausted = true;
                if (OnExhaust != null)
                {
                   
                    OnExhaust(this, EventArgs.Empty);
                }
            }
            else if (value > exhaustThreshold && exhausted)
            {
                if (OnRecover != null)
                {

                    OnRecover(this, EventArgs.Empty);
                }
            }
        }
    }
    public event EventHandler OnExhaust;
    public event EventHandler OnRecover;
    public event Action<int> OnStaminaChange;
    private void Start()
    {
        staminaBar.maxValue = maxStamina;
        staminaBar.value = _stamina;
    }
    private void Update()
    {
        RegenerateStamina();
    }
    void RegenerateStamina()
    {
        if (Stamina < maxStamina)
        {
            timer += (staminaRegenRate * Time.deltaTime);
            if (timer > 1.0f)
            {
                Stamina++;
                timer = 0.0f;
            }
        }
    }
}
