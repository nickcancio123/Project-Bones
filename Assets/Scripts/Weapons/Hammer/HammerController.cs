using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : WeaponController
{
    protected new void Start() => base.Start();   //Base class reference caching

    void Update() => UpdateAnimationParams();
    
    void UpdateAnimationParams()
    {
        if (!weaponAnimator) { return; }

        weaponAnimator.SetFloat("velocity", movementManager.characterController.velocity.magnitude);
        weaponAnimator.SetBool("isRunning", movementManager.isRunning);
    }
}
