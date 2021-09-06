using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MovementModifier
{
    [SerializeField] Stamina staminaComp;

    [SerializeField] float dashDuration = 0.6f;
    [SerializeField] float dashSpeed = 30;
    [SerializeField] float dashCoolDown = 0.4f;
    [SerializeField] float dashStaminaDrain = 33;

    float dashStartTime = 0;
    bool isDashing = false;
    Vector3 dashDirection = Vector3.zero;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    void Start()
    {
        SetModType();
        SetCompatibleModTypes();
    }

    protected override void SetModType()
    {
        modType = EMovementModType.Dash;
    }

    void SetCompatibleModTypes()
    {
        compatibleModTypes.Add(EMovementModType.Force_Receiver);
    }

    void Update() => Move();

    void Move()
    {
        if (!photonView.IsMine) { return; }

        if (CanDash())
        {
            isDashing = true;
            isExclusive = true;
            dashStartTime = Time.time;

            staminaComp.DrainStamina(dashStaminaDrain);

            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            Vector3 forward = movementManager.ownerPlayer.transform.forward;
            Vector3 right = movementManager.ownerPlayer.transform.right;
            dashDirection = right * inputDirection.x + forward * inputDirection.z;
        }

        value = dashDirection * ((isDashing) ? dashSpeed : 0);


        if (isDashing && Time.time - dashStartTime > dashDuration)
        {
            isDashing = false;
            isExclusive = false;
        }
    }

    bool CanDash()
    {
        if (!isDashing && Input.GetKeyDown(KeyCode.LeftAlt))
            if (movementManager.characterController.isGrounded && staminaComp.CanDrainStaminaBy(dashStaminaDrain))
                if (Time.time - dashStartTime - dashDuration > dashCoolDown)
                    return true;

        return false;
    }
}
