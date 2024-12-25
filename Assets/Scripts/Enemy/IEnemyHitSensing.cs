using System;
using UnityEngine;
using UnityEngine.Events;

public interface IEnemyHitSensing
{
    //without physics
    void HitReaction(string animation, int rawDamage, Vector3 hitPoint, Transform incomingLimb);
    //with physics
    void HitReaction(string reactionAnimation, float impulse, Transform incomingLimb, Vector3 hitPoint);

    bool AbleToReceiveHits();

 }
