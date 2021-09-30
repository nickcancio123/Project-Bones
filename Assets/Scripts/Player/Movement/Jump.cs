using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : ForceReceiver
{
    [SerializeField] float jumpForce = 10;
    
    protected override void SetModType() => modType = EMovementModType.Jump;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    public override void ModifyUpdate()
    {
        if (!photonView.IsMine) { return; }
        
        base.ModifyUpdate();
        PollJump();
    }

    void PollJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && movementManager.characterController.isGrounded)
            AddForce(Vector3.up * jumpForce);
    }
}
