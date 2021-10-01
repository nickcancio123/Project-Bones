using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirControl : MovementModifier
{
    [SerializeField] BasicLocomotion basicLocomotion;
    [SerializeField] float acceleration = 1;
    [SerializeField] float drag = 1;

    bool wasGroundedLastFrame = false;
    
    protected override void SetModType() => modType = EMovementModType.Air_Control;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    public override void ModifyUpdate()
    {
        if (!photonView.IsMine) { return; }
        
        if (!movementManager.characterController.isGrounded)
            AirMove();
        else
            value = Vector3.zero;

        wasGroundedLastFrame = movementManager.characterController.isGrounded;
    }

    void AirMove()
    {
        Vector3 vel = movementManager.characterController.velocity;
        Vector3 horizVel = new Vector3(vel.x, 0, vel.z);

        //Initializes value
        if (wasGroundedLastFrame)
        {
            value = horizVel;
            return;
        }
        
        //Get input delta vector
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 right = movementManager.ownerPlayer.transform.right * inputDir.x * acceleration;
        Vector3 forward = movementManager.ownerPlayer.transform.forward * inputDir.z * acceleration;
        Vector3 moveDir = right + forward;
        
        //Limits acceleration in certain direction
        float limiter = 1 - Mathf.Abs(Vector3.Dot(horizVel.normalized, moveDir.normalized));
        limiter = (horizVel.magnitude >= basicLocomotion.GetRunSpeed()) ? limiter : 1;

        value =  Vector3.Lerp(value + moveDir * limiter, Vector3.zero, drag);
    }
}
