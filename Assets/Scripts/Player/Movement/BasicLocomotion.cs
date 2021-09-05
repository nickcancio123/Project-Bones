using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLocomotion : MovementModifier
{
    [SerializeField] float walkSpeed = 5;
    [SerializeField] float runSpeed = 8;
    [SerializeField] float acceleration = 1;

    [SerializeField] float airDrag = 1;
    [SerializeField] float airTurnRate = 1;


    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    void Update()
    {
        if (!photonView.IsMine) { return; }

        if (movementManager.characterController.isGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }
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
        value = Vector3.Lerp(value, targetValue, acceleration);
    }


    void AirMove()
    {
        Vector3 velocity = movementManager.characterController.velocity;
        float currentSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        currentSpeed = Mathf.Lerp(currentSpeed, 0, airDrag * Time.deltaTime);

        Vector3 forward = movementManager.ownerPlayer.transform.forward;

        float forwardInput = Input.GetAxis("Vertical");
        bool givingInput = (Mathf.Abs(forwardInput) > 0.25) ? true : false;

        //Calculates how much one can turn: the faster you turn, the less you can turn
        float turnAngle = Vector3.Angle(value, forward) * Time.deltaTime;
        float turnThrottle = Mathf.Cos(Mathf.Deg2Rad * turnAngle);
        turnThrottle = (turnThrottle < 0) ? 0 : turnThrottle;

        float turnSpeed = airTurnRate * turnThrottle;
        Vector3 lerpedForwardValue = Vector3.Lerp(value.normalized * currentSpeed, forward * currentSpeed, turnSpeed);

        value = givingInput ? lerpedForwardValue : value.normalized * currentSpeed;
    }
}
