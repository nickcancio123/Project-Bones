using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public enum AttackType
{
    Slash,
    Jab
}

public abstract class AttackFeature : WeaponFeature
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


    protected virtual void Attack(GameObject targetPlayer)
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

    public void BeginRecoil()
    {
        Recoil_State recoil_state = gameObject.AddComponent<Recoil_State>();
        TransitionState(activeState, recoil_state);
    }

    protected override void Update() => base.Update();


}
