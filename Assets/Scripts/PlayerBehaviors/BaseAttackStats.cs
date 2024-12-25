using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttackStats: MonoBehaviour
{
    public abstract float ReturnCurrentStamina();
    //Stamina costs
    public Dictionary<string, int> StaminaCosts = new Dictionary<string, int>()
    {
        {"Cross", 10},
        {"JabCommand", 4},
        {"JabCrossCommand",15 },
        {"Dodge",5 },
        {"ChainPunchCommand",25 },
        {"DodgeBackCommand", 5},
        {"DodgeRightCommand", 5},
        {"DodgeLeftCommand", 5}
    };
}
