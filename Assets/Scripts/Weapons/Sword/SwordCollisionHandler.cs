using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionHandler : MonoBehaviour
{
    [SerializeField] SwordAttackFeature swordAttackScript;

    private void OnTriggerEnter(Collider other)
    {
        swordAttackScript.OnSwordCollision(other);
    }
}
