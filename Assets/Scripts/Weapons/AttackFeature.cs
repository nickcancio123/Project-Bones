using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public enum AttackType
{
    Slash,
    Jab
}

public abstract class AttackFeature : WeaponFeature, IPunObservable
{
    #region Public Interface
    public AttackType attackType;
    [SerializeField] protected float attackDamage = 1;
    [HideInInspector] public float attackAngle = 0;
    #endregion

    //Can be implemented by individual features to stop default behavior
    protected bool stopDefaultBehavior = false;

    protected void Attack(GameObject targetPlayer)
    {
        //Override and call base to add functionality
        Health healthComponent = targetPlayer.GetComponent<Health>();
        healthComponent?.TakeWeaponDamage(attackDamage, photonView.ViewID);
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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


    #region Recoil Methods
    [SerializeField] protected float recoilDuration = 1;

    protected bool isRecoiling = false;

    protected float recoilStartTime = 0;
    protected Vector3 recoilStartLocalPos;
    protected Quaternion recoilStartLocalRotation;

    protected void BeginRecoilFromBlock()
    {
        recoilStartLocalPos = transform.localPosition;
        recoilStartLocalRotation = transform.localRotation;
        recoilStartTime = Time.time;

        stopDefaultBehavior = true;
        isRecoiling = true;
    }

    protected abstract void RecoilFromBlock();

    protected void StopRecoiling()
    {
        stopDefaultBehavior = false;
        isRecoiling = false;
    }
    #endregion
}
