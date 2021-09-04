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
        if (!weaponAnimator)
        {
            print("No weapon animator ref");
            return;
        }

        weaponAnimator.SetFloat("velocity", movementManager.GetSpeed());
        weaponAnimator.SetBool("isRunning", movementManager.isRunning);
    }
}
