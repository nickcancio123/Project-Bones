using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
>Goes on the hand game objects of Skely
>Finds the transforms of the grips on the weapon and places the hands on the corresponding grips
*/

public class HandPlacement : MonoBehaviour
{
    [SerializeField] GameObject ownerPlayer;
    [SerializeField] bool isLeftHand = true;

    Transform grip;
    
    void LateUpdate()
    {
        if (!grip)
        {
            IWeapon weapon = ownerPlayer?.GetComponentInChildren<IWeapon>(true);
            if (!weapon)
                return;
            grip = isLeftHand ? weapon.leftGrip : weapon.rightGrip;
        }

        this.transform.position = grip.position;
    }
}
