using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer_HeavySlash_Swing_State : Slash_Swing_State
{
    HammerHeavySlashFeature wFeature;
    
    public override void BeginState()
    {
        base.BeginState();
        wFeature = (HammerHeavySlashFeature) wController.GetActiveFeatureAsComponent();
        wFeature.canImpactGround = true;
    }

    protected override void EndState()
    {
        base.EndState();
        wFeature.canImpactGround = false;
    }
}
