using System.Collections;
using System.Collections.Generic;
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

    public void ApplyKnockBack(float force, Vector3 direction)
    {
        if (photonView.IsMine)
            AddForce(force * direction);
    }
}
