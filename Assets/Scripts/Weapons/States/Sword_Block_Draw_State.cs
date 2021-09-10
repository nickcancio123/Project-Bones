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

    void Awake()
    {
        wFeature = GetComponent<SwordBlockFeature>();
        wController = wFeature.weaponController;
        drawDuration = wFeature.drawDuration;
        targetLocalPos = wFeature.defaultBlockPosition;
        targetLocalRotation = wFeature.defaultBlockRotation;
    }

    public override void BeginState()
    {
        wController.DisableAnimator();
        wController.skelyMovement.canRotate = false;
        drawStartTime = Time.time;
    }

    protected override void EndState()
    {
        Sword_Block_State sword_block_state = gameObject.AddComponent<Sword_Block_State>();
        wFeature.TransitionState(this, sword_block_state);
    }

    public override void Behave()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration;

        //Draw to position
        Transform skelyTransform = wController.ownerSkely.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(wController.defaultPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);

        //Draw to rotation
        Quaternion initialRotation = Quaternion.Euler(wController.defaultRotation);
        Quaternion targetRotation = Quaternion.Euler(targetLocalRotation);
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, drawProgress);

        //Once sword is drawn
        if (drawProgress > 1)
        {
            EndState();
        }
    }
}
