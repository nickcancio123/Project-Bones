using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_Swing_State : FeatureState
{
    SlashAttackFeature wFeature;
    WeaponController wController;
    Vector3 mouseSwipe = Vector3.zero;

    float slashDistance = 0;
    float slashAngle = 0;
    float slashDuration = 0;

    float slashStartTime = 0;
    Vector3 drawnLocalPosition = Vector3.zero;

    void Awake()
    {
        wFeature = GetComponent<SlashAttackFeature>();
        wController = wFeature.weaponController;

        slashDistance = wFeature.slashDistance;
        slashAngle = wFeature.slashAngle;
        slashDuration = wFeature.slashDuration;
    }

    public override void BeginState()
    {
        slashStartTime = Time.time;
        drawnLocalPosition = transform.localPosition;
        wFeature.canDealDamage = true;
        wFeature.slashTrails.SetActive(true);
    }

    protected override void EndState()
    {
        wFeature.canDealDamage = false;
        wFeature.slashTrails.SetActive(false);

        FeatureState reset_state = gameObject.AddComponent<Reset_State>();
        wFeature.TransitionState(this, reset_state);
    }

    public void Initialize(Vector3 _mouseSwipe)
    {
        mouseSwipe = _mouseSwipe;
    }

    public override void Behave()
    {
        float slashProgress = (Time.time - slashStartTime) / slashDuration; //0-1

        SlashToPosition(slashProgress);
        SlashToRotation();

        //When done slash
        if (slashProgress > 1)
        {
            EndState();
        }
    }

    void SlashToPosition(float slashProgress)
    {
        //Calculate target world position
        Vector3 localDelta = new Vector3(mouseSwipe.x * slashDistance, mouseSwipe.y * slashDistance, 0);
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
        float rotateDirection = (mouseSwipe.x > 0) ? -1 : 1;
        float rotationRate = (Time.deltaTime * slashAngle) / slashDuration;
        float deltaX = rotateDirection * rotationRate;

        //Rotate deltaX per frame
        transform.Rotate(new Vector3(deltaX, 0, 0), Space.Self);
    }
}
