using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
>Goes on the hand game objects of Skely
>Finds the transforms of the grips on the weapon and places the hands on the corresponding grips
*/

public class HandPlacement : MonoBehaviour
{
    [SerializeField] GameObject ownerSkely;
    [SerializeField] bool isLeftHand = true;

    Transform grip;

    void Start()
    {
        if (!ownerSkely)
        {
            print("No ownerSkely ref");
            return;
        }

        IWeapon weapon = ownerSkely.GetComponentInChildren<IWeapon>(true);
        grip = isLeftHand ? weapon.leftGrip : weapon.rightGrip;
    }

    void LateUpdate()
    {
        if (!grip)
        {
            print("No grip ref");
            return;
        }

        this.transform.position = grip.position;
    }
}
