using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum EMovementModType
{
    Basic_Locomotion,
    Dash,
    Force_Receiver,
    Gravity,
    Jump
}

public abstract class MovementModifier : MonoBehaviourPunCallbacks
{
    protected MovementManager movementManager;

    void Awake()
    {
        movementManager = GetComponent<MovementManager>();
    }


    protected Vector3 value = Vector3.zero;

    public Vector3 GetValue() { return value; }



    //If a modifier is exclusive, it will be the only modifier to affect player 
    //movement with exception to modifiers of types compatibleModTypes
    protected bool isExclusive = false;
    public bool IsExclusive() => isExclusive;
    [HideInInspector] public List<EMovementModType> compatibleModTypes = new List<EMovementModType>();

    protected EMovementModType modType;
    public EMovementModType GetModType() => modType;
    protected abstract void SetModType();
}
