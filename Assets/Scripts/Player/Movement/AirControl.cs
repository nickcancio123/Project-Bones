using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirControl : MovementModifier
{
    [SerializeField] float maxSpeedToAccel = 10;
    [SerializeField] float acceleration = 1;
    [SerializeField] float drag = 1;
    
    
    protected override void SetModType() => modType = EMovementModType.Air_Control;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    void Update()
    {
        if (!photonView.IsMine) { return; }
        
        if (!movementManager.characterController.isGrounded)
            AirMove();
        else
            value = Vector3.zero;
    }

    void AirMove()
    {
        Vector3 vel = movementManager.characterController.velocity;
        Vector3 horizVel = new Vector3(vel.x, 0, vel.z);
        
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        
        Vector3 right = movementManager.ownerPlayer.transform.right * inputDir.x * acceleration;
        Vector3 forward = movementManager.ownerPlayer.transform.forward * inputDir.z * acceleration;
        Vector3 moveDir = right + forward;
        
        horizVel = Vector3.Lerp(horizVel, Vector3.zero, drag);

        float limiter = 1 - Mathf.Abs(Vector3.Dot(horizVel.normalized, moveDir.normalized));
        limiter = (horizVel.magnitude >= maxSpeedToAccel) ? limiter : 1;
        value = horizVel + moveDir * limiter;
    }
}
