using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLocomotion : MovementModifier
{
    [SerializeField] Stamina staminaComp;

    [Header("Ground Controls")]
    [SerializeField] float walkSpeed = 5;
    [SerializeField] float runSpeed = 8;
    [SerializeField] float backwardRatio = 0.7f;
    [SerializeField] float runStaminaDrain = 10; //Per second
    [SerializeField] float acceleration = 1;

    [Header("Air Controls")]
    [SerializeField] float airDrag = 1;
    [SerializeField] float airAccel = 1;


    void Start() => SetModType();

    protected override void SetModType() => modType = EMovementModType.Basic_Locomotion;

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
        inputDirection.y = Mathf.Clamp(inputDirection.y, -backwardRatio, 1);
        
        movementManager.isRunning = Input.GetKey(KeyCode.LeftShift) && CanRun();

        if (movementManager.isRunning && movementManager.characterController.velocity.magnitude > 4)
            staminaComp.DrainStamina(runStaminaDrain * Time.deltaTime);

        float targetSpeed = (movementManager.isRunning ? runSpeed : walkSpeed);
        targetSpeed = (inputDirection.magnitude == 0) ? 0 : targetSpeed;

        //Get desired delta vector
        Vector3 forward = movementManager.ownerPlayer.transform.forward * inputDirection.y;
        Vector3 right = movementManager.ownerPlayer.transform.right * inputDirection.x;
        Vector3 moveDirection = forward + right;

        Vector3 targetValue = targetSpeed * moveDirection;
        value = Vector3.Lerp(value, targetValue, acceleration);
    }

    bool CanRun() => staminaComp.CanDrainStaminaBy(runStaminaDrain * Time.deltaTime);
    
    void AirMove()
    {
        Vector3 velocity = movementManager.characterController.velocity;
        float currentSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, runSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, 0, airDrag * Time.deltaTime);

        Vector3 forward = movementManager.ownerPlayer.transform.forward;
        Vector3 right = movementManager.ownerPlayer.transform.right;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 moveDelta = (input.x * right) + (input.z * forward);
        
        value = Vector3.Lerp(value, Vector3.zero, airDrag);
        
        value = (value.normalized * currentSpeed) +  (moveDelta * airAccel);
    }
}
