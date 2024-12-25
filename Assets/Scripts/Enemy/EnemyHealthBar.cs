using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthBar;
    public int maxHealth = 100;
    public int nearDeathThreshold = 10;
  [SerializeField]  private int _health;
    public int Health
    {

        get { return _health; }
        set { _health = value;
            healthBar.value = _health;
            if (OnHealthChange != null)
            { OnHealthChange(value); }
            if (value <= nearDeathThreshold && value>0)
            {
                if (OnNearDeath != null)
                    OnNearDeath(this, EventArgs.Empty);
            }
           else if (value <= 0) 
            {
                if (OnDeath != null)
                {
                    OnDeath(this,EventArgs.Empty);
                }
            }
        }
    }
    public event EventHandler OnDeath;
    public event EventHandler OnNearDeath;
    public event Action<int> OnHealthChange;
    private void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = _health;
    }
}
