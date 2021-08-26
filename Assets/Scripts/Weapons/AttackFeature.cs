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
    #region Public Interface
    public AttackType attackType;
    [SerializeField] protected float attackDamage = 1;
    [HideInInspector] public float attackAngle = 0;
    #endregion


    protected void DealDamage(GameObject targetPlayer)
    {
        IHealth healthComponent = targetPlayer.GetComponent<IHealth>();
        healthComponent?.TakeWeaponDamage(attackDamage, photonView.ViewID);
    }
}
