using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackStats : BaseAttackStats
{
    PlayerStateMachine _player;
    public override float ReturnCurrentStamina()
    {
        return _player.Stamina;
    }
    private void Start()
    {
        _player = GetComponent<PlayerStateMachine>();
    }
}
