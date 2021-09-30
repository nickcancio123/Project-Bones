using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReceiver : MovementModifier
{
    [SerializeField] protected float mass = 100;
    [SerializeField] protected float drag = 1;
    
    
    protected override void SetModType() => modType = EMovementModType.Force_Receiver;

    new void OnEnable() => movementManager.AddModifier(this);
    new void OnDisable() => movementManager.RemoveModifier(this);

    protected virtual void Update()
    {
        if (!photonView.IsMine) { return; }

        //Cutoff value to zero
        if (value.magnitude < 0.25)
        {
            value = Vector3.zero;
        }

        //Implement drag
        value = Vector3.Lerp(value, Vector3.zero, drag * Time.deltaTime);
    }

    protected void AddForce(Vector3 force) => value += force / mass;
}
