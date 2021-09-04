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

        //Cutoff value to zero
        if (value.magnitude < 0.25)
        {
            value = Vector3.zero;
        }

        //Implement drag
        value = Vector3.Lerp(value, Vector3.zero, drag * Time.deltaTime);

        wasGroundedLastFrame = movementManager.characterController.isGrounded;
    }

    public void AddForce(Vector3 force) => value += force / mass;
}
