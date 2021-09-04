using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReceiver : MovementModifier
{
    [SerializeField] float mass = 100;
    [SerializeField] float drag = 1;

    bool wasGroundedLastFrame = false;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    void Update()
    {
        if (!photonView.IsMine) { return; }

        // if (!wasGroundedLastFrame && movementManager.characterController.isGrounded)
        // {
        //     value = new Vector3(value.x, 0, value.z);
        // }

        //Cutoff value to zero
        if (value.magnitude < 0.1)
        {
            value = Vector3.zero;
        }

        //Implement drag
        value = Vector3.Lerp(value, Vector3.zero, drag * Time.deltaTime);
    }

    public void AddForce(Vector3 force) => value += force / mass;
}
