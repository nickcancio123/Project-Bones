using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLocomotion : MovementModifier
{
    [SerializeField] float walkSpeed = 5;
    [SerializeField] float runSpeed = 8;
    [SerializeField] float groundAcceleration = 1;
    [SerializeField] float airDrag = 1;


    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    void Update()
    {
        if (!photonView.IsMine) { return; }

        if (movementManager.characterController.isGrounded)
            GroundMove();
        else
            AirMove();
    }

    void GroundMove()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;

        movementManager.isRunning = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = (movementManager.isRunning ? runSpeed : walkSpeed);
        targetSpeed = (inputDirection.magnitude == 0) ? 0 : targetSpeed;

        Vector3 forward = movementManager.ownerPlayer.transform.forward;
        Vector3 right = movementManager.ownerPlayer.transform.right;
        Vector3 moveDirection = forward * inputDirection.y + right * inputDirection.x;

        Vector3 targetValue = targetSpeed * moveDirection;
        value = Vector3.Lerp(value, targetValue, groundAcceleration);
    }

    void AirMove()
    {
        value = Vector3.Lerp(value, Vector3.zero, airDrag * Time.deltaTime);
    }
}
