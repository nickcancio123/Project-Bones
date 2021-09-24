using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_Swing_State : FeatureState
{
    SlashAttackFeature wFeature;
    WeaponController wController;
    Vector3 slashDirection = Vector3.zero;

    float slashDistance = 0;
    float slashAngle = 0;
    float slashDuration = 0;

    float slashStartTime = 0;
    Vector3 drawnLocalPosition = Vector3.zero;


    protected override void CreateNextState() => nextState = gameObject.AddComponent<Reset_State>();

    public override void BeginState()
    {
        Initialize();
        
        slashStartTime = Time.time;
        drawnLocalPosition = transform.localPosition;
        wFeature.canDealDamage = true;
        wFeature.slashTrails.SetActive(true);
    }

    protected override void EndState()
    {
        wFeature.canDealDamage = false;
        wFeature.slashTrails.SetActive(false);
        
        TransitionState();
    }

    public override void Behave()
    {
        float slashProgress = (Time.time - slashStartTime) / slashDuration; //0-1

        SlashToPosition(slashProgress);
        SlashToRotation();

        if (slashProgress > 1)
            EndState();
    }

    void Initialize()
    {
        wFeature = (SlashAttackFeature) GetComponent<WeaponController>()?.GetActiveFeatureAsComponent();
        wController = wFeature.weaponController;

        slashDirection = wFeature.slashDirection;
        slashDistance = wFeature.slashDistance;
        slashAngle = wFeature.slashAngle;
        slashDuration = wFeature.slashDuration;
    }
    
    void SlashToPosition(float slashProgress)
    {
        //Calculate target world position
        Vector3 localDelta = new Vector3(slashDirection.x * slashDistance, slashDirection.y * slashDistance, 0);
        Vector3 targetLocalPos = wController.defaultPosition + localDelta;
        Transform playerTransform = wController.ownerPlayer.transform;
        Vector3 targetWorldPos = playerTransform.position + playerTransform.TransformDirection(targetLocalPos);

        //Lerp position
        Vector3 startingPos = playerTransform.position + playerTransform.TransformDirection(drawnLocalPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, slashProgress);
    }

    void SlashToRotation()
    {
        //Calculate rotation about local X-axis
        float rotateDirection = (slashDirection.x >= 0) ? -1 : 1;
        float rotationRate = (Time.deltaTime * slashAngle) / slashDuration;
        float deltaX = rotateDirection * rotationRate;

        //Rotate deltaX per frame
        transform.Rotate(new Vector3(deltaX, 0, 0), Space.Self);
    }
}
