using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Block_Draw_State : Draw_State
{
    SwordBlockFeature wFeature;

    protected override void CreateNextState() => nextState = gameObject.AddComponent<Sword_Block_State>();

    public override void BeginState()
    {
        base.BeginState();
        wController.movementManager.canRotate = false;
    }
    
    protected override void Initialize()
    {
        wFeature = GetComponent<SwordBlockFeature>();
    }

    protected override void SetTargetLocalPosition() => targetLocalPos = wFeature.defaultBlockPosition;
    protected override void SetTargetLocalRotation() => targetLocalRotation = Quaternion.Euler(wFeature.defaultBlockRotation);
    protected override void SetDrawDuration() => drawDuration = wFeature.drawDuration;
}
