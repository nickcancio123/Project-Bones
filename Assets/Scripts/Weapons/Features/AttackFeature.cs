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
    [Header("Attack")]
    public AttackType attackType;
    public float recoilDuration = 1;
    [SerializeField] protected float attackDamage = 1;
    
    [HideInInspector] public float attackAngle = 0;
    [HideInInspector] public Vector3 drawnLocalPos;
    [HideInInspector] public Quaternion drawnLocalRotation;
    
    [HideInInspector] public bool canDealDamage = false;
    #endregion

    protected override void Update() => base.Update();

    protected virtual void Attack(GameObject targetPlayer)
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
    

    #region Recoil from block
    [PunRPC]
    public void RPC_BeginRecoil(float knockBackForce) => BeginRecoil(knockBackForce);

    protected virtual void BeginRecoil(float knockBackForce)
    {
        if (!photonView.IsMine)
            return;
        
        //Set state
        canDealDamage = false;

        //Recoil
        Recoil_State recoil_state = gameObject.AddComponent<Recoil_State>();
        TransitionState(activeState, recoil_state);
        
        //Knock back from block
        KnockBack knockBackComp = weaponController.ownerPlayer.GetComponent<KnockBack>();
        if (knockBackComp)
        {
            Vector3 force = knockBackForce * -weaponController.ownerPlayer.transform.forward;            
            knockBackComp.TakeKnockBack(force);
        }
    }
    #endregion

}
