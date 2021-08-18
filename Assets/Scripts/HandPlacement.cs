using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
        if (!grip)
        {
            print("No grip ref");
            return;
        }

        this.transform.position = grip.position;
    }
}
