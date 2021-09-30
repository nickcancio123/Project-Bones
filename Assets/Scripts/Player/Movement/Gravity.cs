using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MovementModifier
{
    [SerializeField] float groundedPullForce = 10;

    readonly float gravityMagnitude = Physics.gravity.y;
    bool wasGroundedLastFrame = false;
    
    protected override void SetModType() => modType = EMovementModType.Gravity;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    public override void ModifyUpdate() => ApplyGravity();

    void ApplyGravity()
    {
        if (!photonView.IsMine) { return; }

        if (movementManager.characterController.isGrounded)
        {
            value = new Vector3(0, -groundedPullForce, 0);
        }
        else if (wasGroundedLastFrame)
        {
            //On just left ground, reset gravity
            value = Vector3.zero;
        }
        else
        {
            value = new Vector3(0, value.y + gravityMagnitude * Time.deltaTime, 0);
        }

        wasGroundedLastFrame = movementManager.characterController.isGrounded;
    }
}
