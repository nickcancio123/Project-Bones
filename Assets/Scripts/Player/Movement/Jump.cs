using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MovementModifier
{
    [SerializeField] float jumpForce = 10;

    void Start() => SetModType();

    protected override void SetModType() => modType = EMovementModType.Jump;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    void Update()
    {
        PollJump();
    }

    void PollJump()
    {
        if (!photonView.IsMine) { return; }

        if (Input.GetKeyDown(KeyCode.Space) && movementManager.characterController.isGrounded)
        {
            ForceReceiver forceReceiver = GetComponent<ForceReceiver>();
            forceReceiver?.AddForce(Vector3.up * jumpForce);
        }
    }
}
