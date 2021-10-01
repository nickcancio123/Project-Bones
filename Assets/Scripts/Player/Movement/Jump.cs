using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : ForceReceiver
{
    [SerializeField] float jumpForce = 10;

    void Start() => SetModType();

    protected override void SetModType() => modType = EMovementModType.Jump;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    protected override void Update()
    {
        if (!photonView.IsMine) { return; }
        base.Update();
        PollJump();
    }

    void PollJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && movementManager.characterController.isGrounded)
            AddForce(Vector3.up * jumpForce);
    }
}
