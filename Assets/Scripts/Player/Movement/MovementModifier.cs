using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class MovementModifier : MonoBehaviourPunCallbacks
{
    protected MovementManager movementManager;

    void Awake()
    {
        movementManager = GetComponent<MovementManager>();
    }

    protected Vector3 value = Vector3.zero;

    public Vector3 GetValue() { return value; }
}
