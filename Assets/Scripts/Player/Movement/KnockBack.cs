using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KnockBack : ForceReceiver
{
    protected override void SetModType() => modType = EMovementModType.Knock_Back;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);


    public override void ModifyUpdate()
    {
        if (!photonView.IsMine) { return; }
        base.ModifyUpdate();
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
