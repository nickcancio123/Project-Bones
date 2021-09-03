using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollisionHandler : MonoBehaviour
{
    [SerializeField] WeaponController weaponController;

    private void OnTriggerEnter(Collider other)
    {
        weaponController.OnWeaponCollision(other);
    }
}
