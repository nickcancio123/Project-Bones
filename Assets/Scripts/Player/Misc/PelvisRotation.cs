using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelvisRotation : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject ownerPlayer;

    void Update()
    {
        Vector3 velocity = characterController.velocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        //Turn pelvis when not moving
        if (horizontalVelocity.magnitude > 0.1f)
        {
            transform.forward = horizontalVelocity.normalized;
        }
        else
        {
            transform.forward = Vector3.Lerp(transform.forward, ownerPlayer.transform.forward, 0.5f);
        }
    }
}
