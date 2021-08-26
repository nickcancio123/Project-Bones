using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionHandler : MonoBehaviour
{
    [SerializeField] SwordAttackFeature swordAttackScript;

    private void OnTriggerEnter(Collider other)
    {
        print("On trigger enter");
        swordAttackScript.OnSwordCollision(other);
    }
}
