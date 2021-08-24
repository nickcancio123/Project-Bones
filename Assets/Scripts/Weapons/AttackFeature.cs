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
    public AttackType attackType;

    void DealDamage(float maxDamageAmount, GameObject targetPlayer)
    {
        IHealth healthComponent = targetPlayer.GetComponent<IHealth>();

        if (!healthComponent)
        {
            print("No health component found!");
            return;
        }

        //Call health component's TakeDamage RPC
        GameObject attacker = weaponController.ownerSkely;
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, this, maxDamageAmount, attacker);
    }
}
