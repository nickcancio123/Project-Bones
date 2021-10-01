using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Dash : MovementModifier, IPunObservable
{
    [SerializeField] Stamina staminaComp;
    [SerializeField] GameObject dashTrails;

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

        if (CanDash())  //Start dash
        {
            isDashing = true;
            isExclusive = true;
            dashStartTime = Time.time;

            staminaComp.DrainStamina(dashStaminaDrain);

            dashTrails.SetActive(true);
            StartCoroutine(StopDashTrails());

            //Calculate dash direction
            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            Vector3 forward = movementManager.ownerPlayer.transform.forward;
            Vector3 right = movementManager.ownerPlayer.transform.right;

            if (inputDirection.magnitude > 0.1)
                dashDirection = right * inputDirection.x + forward * inputDirection.z;
            else
                dashDirection = forward;
        }

        value = dashDirection * ((isDashing) ? dashSpeed : 0);


        if (isDashing && Time.time - dashStartTime > dashDuration)
        {
            isDashing = false;
            isExclusive = false;
            //dashTrails.SetActive(false);
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

    void SetupDashTrails()
    {
        if (!dashTrails) { return; }

        dashTrails.SetActive(false);
    }

    IEnumerator StopDashTrails()
    {
        yield return new WaitForSeconds(dashDuration + dashCoolDown - 0.05f);
        //yield return new WaitForSeconds(1);
        dashTrails.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(dashTrails.activeInHierarchy);
        }
        else
        {
            if (!photonView.IsMine)
                dashTrails.SetActive((bool)stream.ReceiveNext());
        }
    }
}
