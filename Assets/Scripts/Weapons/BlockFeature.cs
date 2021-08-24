using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class BlockFeature : WeaponFeature
{
    //Returns damage to be taken
    //If not implemented, damage taken is maxDamage
    public float BlockAttack(AttackFeature attackScript, float maxDamageAmount, GameObject attacker)
    {
        return maxDamageAmount;
    }
}
