using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] MovementManager movementManager;

    void Update()
    {
        UpdateAnimationParams();
    }

    void UpdateAnimationParams()
    {
        if (!playerAnimator)
        {
            print("No player animator ref");
            return;
        }

        playerAnimator.SetFloat("velocity", movementManager.GetSpeed());
        playerAnimator.SetBool("isRunning", movementManager.isRunning);
    }
}
