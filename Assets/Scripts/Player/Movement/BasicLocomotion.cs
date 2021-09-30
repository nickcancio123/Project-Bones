using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLocomotion : MovementModifier
{
    [SerializeField] Stamina staminaComp;

    [SerializeField] float walkSpeed = 5;
    [SerializeField] float runSpeed = 8;
    [SerializeField] float backwardRatio = 0.7f;
    [SerializeField] float runStaminaDrain = 10; //Per second
    [SerializeField] float acceleration = 1;

    bool wasGroundedLastFrame = false;

    protected override void SetModType() => modType = EMovementModType.Basic_Locomotion;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    public override void ModifyUpdate()
    {
        if (!photonView.IsMine) { return; }
        
        if (movementManager.characterController.isGrounded)
            GroundMove();
        else
            value = Vector3.zero;

        wasGroundedLastFrame = movementManager.characterController.isGrounded;
    }

    void GroundMove()
    {
        if (!wasGroundedLastFrame)
        {
            OnLanding();
            return;
        }
        
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        inputDir.y = Mathf.Clamp(inputDir.y, -backwardRatio, 1);
        
        movementManager.isRunning = Input.GetKey(KeyCode.LeftShift) && CanRun();

        //Drain stamina
        if (movementManager.isRunning && movementManager.characterController.velocity.magnitude > walkSpeed)
            staminaComp.DrainStamina(runStaminaDrain * Time.deltaTime);
        
        //Get desired move speed
        float targetSpeed = (movementManager.isRunning ? runSpeed : walkSpeed);
        targetSpeed = (inputDir.magnitude == 0) ? 0 : targetSpeed;
        
        //Transform desired move direction to world coords
        Vector3 forward = movementManager.ownerPlayer.transform.forward * inputDir.z;
        Vector3 right = movementManager.ownerPlayer.transform.right * inputDir.x;
        Vector3 moveDirection = forward + right;
        
        Vector3 targetValue = targetSpeed * moveDirection;
        value = Vector3.Lerp(value, targetValue, acceleration);
    }

    void OnLanding()
    {
        Vector3 velocity = movementManager.characterController.velocity;
        value = new Vector3(velocity.x, 0, velocity.z);
    }
    
    bool CanRun() => staminaComp.CanDrainStaminaBy(runStaminaDrain * Time.deltaTime);
}

