using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class BlockFeature : WeaponFeature
{
    protected bool isBlocking = false;
    public bool IsBlocking() { return isBlocking; }

    //Returns damage to be taken
    //If not implemented, damage taken is maxDamage
    public virtual float BlockAttack(float maxDamageAmount, int attackerID)
    {
        return maxDamageAmount;
    }
}
