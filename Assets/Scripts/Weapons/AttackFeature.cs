using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public enum AttackType
{
    Swipe,
    Jab
}

public class AttackFeature : WeaponFeature
{
    [SerializeField] protected float attackDamage = 1;


    public AttackType attackType;


    //Attack information
    [HideInInspector] public float attackAngle = 0;


    protected void DealDamage(float maxDamageAmount, GameObject targetPlayer)
    {
        IHealth healthComponent = targetPlayer.GetComponent<IHealth>();

        if (!healthComponent)
        {
            print("No health component found!");
            return;
        }

        healthComponent.TakeWeaponDamage(maxDamageAmount, photonView.ViewID);
    }
}
