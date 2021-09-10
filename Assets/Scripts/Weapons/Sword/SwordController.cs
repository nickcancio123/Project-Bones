using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordController : WeaponController
{
    new protected void Start()
    {
        base.Start();   //Handles base class reference caching
    }

    void Update()
    {
        UpdateAnimationParams();
    }

    void UpdateAnimationParams()
    {
        if (!weaponAnimator) { return; }

        weaponAnimator.SetFloat("velocity", movementManager.characterController.velocity.magnitude);
        weaponAnimator.SetBool("isRunning", movementManager.isRunning);
    }
}
