using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_Draw_State : Draw_State
{
    SlashAttackFeature wFeature;

    float drawDistance = 0;
    float drawAngle = 0;
    Vector3 slashDirection;

    Vector3 drawnLocalPosition;
    Quaternion drawnLocalRotation;
    
    
    protected override void CreateNextState() => nextState = gameObject.AddComponent<Slash_Swing_State>();

    public override void BeginState()
    {
        base.BeginState();
        wFeature.attackAngle = CalculateTargetSlashAngle();
        wFeature.drawnLocalPos = targetLocalPos;
        wFeature.drawnLocalRotation = targetLocalRotation;
    }
    
    protected override void Initialize()
    {
        wFeature = (SlashAttackFeature) GetComponent<WeaponController>()?.GetActiveFeatureAsComponent();
        drawDistance = wFeature.drawDistance;
        drawAngle = wFeature.drawAngle;
        slashDirection = wFeature.slashDirection;
    }

    protected override void SetTargetLocalPosition()
    {
        //Draw sword in opposite direction as slashDirection
        Vector3 drawDir = new Vector3(-slashDirection.x, -slashDirection.y, 0);
        Vector3 localDelta = new Vector3(drawDir.x * drawDistance, drawDir.y * drawDistance, 0);
        targetLocalPos = wController.defaultPosition + localDelta;
    }

    protected override void SetTargetLocalRotation()
    {
        //Draw sword in opposite direction as slashDirection
        Vector3 drawDir = new Vector3(-slashDirection.x, slashDirection.y, 0); 
        
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        float targetZAngle = CalculateTargetSlashAngle();
        
        Quaternion deltaRotation =
            Quaternion.Euler(drawDir.y * drawAngle,drawDir.x * drawAngle, targetZAngle);

        targetLocalRotation = zeroRotation * deltaRotation;
    }

    protected override void SetDrawDuration() => drawDuration = wFeature.drawDuration;
    
    
    float CalculateTargetSlashAngle()
    {
        //Only half of possible angles need to be accounted for because they are functionally the same
        //If sign of x is negative, flip signs of x and y
        float signAdjustment = (slashDirection.x < 0) ? -1 : 1;
        Vector3 adjustedSlashDir = signAdjustment * slashDirection;
        float targetZAngle = Mathf.Rad2Deg * Mathf.Atan(adjustedSlashDir.y / adjustedSlashDir.x);
        targetZAngle -= 90.0f;
        return targetZAngle;
    }
}
