using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        weaponAnimator.SetFloat("velocity", skelyMovement.velocity);
        weaponAnimator.SetBool("isRunning", skelyMovement.isRunning);
    }
}
