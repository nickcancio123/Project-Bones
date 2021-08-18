using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttackFeature : WeaponFeature
{
    void Start()
    {

    }

    void Update()
    {
        if (featureState != EFeatureState.Disabled)
        {
            ReadInput();
        }
    }

    protected override void ReadInput()
    {

    }
}
