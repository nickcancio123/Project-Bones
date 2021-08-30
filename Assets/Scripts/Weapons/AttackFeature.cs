using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public enum AttackType
{
    Slash,
    Jab
}

public class AttackFeature : WeaponFeature
{
    #region Public Interface
    public AttackType attackType;
    [SerializeField] protected float attackDamage = 1;
    [HideInInspector] public float attackAngle = 0;
    #endregion

    protected void Attack(GameObject targetPlayer)
    {
        //Override and call base to add functionality
        Health healthComponent = targetPlayer.GetComponent<Health>();
        healthComponent?.TakeWeaponDamage(attackDamage, photonView.ViewID);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(attackAngle);
        }
        else
        {
            attackAngle = (float)stream.ReceiveNext();
        }
    }
}
