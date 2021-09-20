using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Block_Draw_State : FeatureState
{
    SwordBlockFeature wFeature;
    WeaponController wController;

    float drawDuration = 0;
    Vector3 targetLocalPos;
    Vector3 targetLocalRotation;

    float drawStartTime = 0;
    
    protected override void CreateNextState() => nextState = gameObject.AddComponent<Sword_Block_State>();

    public override void BeginState()
    {
        Initialize();
        
        wController.DisableAnimator();
        wController.movementManager.canRotate = false;
        drawStartTime = Time.time;
    }

    protected override void EndState() => TransitionState();

    public override void Behave()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration;

        //Draw to position
        Transform skelyTransform = wController.ownerPlayer.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(wController.defaultPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);

        //Draw to rotation
        Quaternion initialRotation = Quaternion.Euler(wController.defaultRotation);
        Quaternion targetRotation = Quaternion.Euler(targetLocalRotation);
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, drawProgress);

        if (drawProgress > 1)
            EndState();
    }

    void Initialize()
    {
        wFeature = GetComponent<SwordBlockFeature>();
        wController = wFeature.weaponController;
        drawDuration = wFeature.drawDuration;
        targetLocalPos = wFeature.defaultBlockPosition;
        targetLocalRotation = wFeature.defaultBlockRotation;
    }
}
