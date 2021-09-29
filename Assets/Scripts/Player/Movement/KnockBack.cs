using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KnockBack : ForceReceiver
{
    void Start() => SetModType();

    protected override void SetModType() => modType = EMovementModType.Jump;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);


    protected override void Update()
    {
        if (!photonView.IsMine) { return; }
        base.Update();
    }

    public void TakeKnockBack(Vector3 force, bool unblockable)
    {
        photonView.RPC("RPC_TakeKnockBack", RpcTarget.All, force, unblockable);
    }

    [PunRPC]
    void RPC_TakeKnockBack(Vector3 force, bool unblockable)
    {
        if (!photonView.IsMine)
            return;

        if (!unblockable)
        {
            GameObject weapon = gameObject.GetComponentInChildren<IWeapon>()?.gameObject;
            BlockFeature blockFeature = weapon?.GetComponent<BlockFeature>();
            if (blockFeature)
                if (blockFeature.isBlocking)
                    return;
        }
        
        AddForce(force);
    }
}
