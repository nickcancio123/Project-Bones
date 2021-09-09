using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_Draw_State : FeatureState
{
    SlashAttackFeature wFeature;

    float drawDistance = 0;
    float drawAngle = 0;
    float drawDuration = 0;

    Vector3 mouseSwipe;
    float drawStartTime = 0;

    Vector3 drawnLocalPosition;
    Quaternion drawnLocalRotation;


    void Awake()
    {
        drawDistance = wFeature.drawDistance;
        drawAngle = wFeature.drawAngle;
        drawDuration = wFeature.drawDuration;
    }

    public override void BeginState()
    {
        wFeature.weaponController.DisableAnimator();
        mouseSwipe.Normalize();
        drawStartTime = Time.time;
    }

    protected override void EndState()
    {
        Slash_Swing_State slash_swing_state = gameObject.AddComponent<Slash_Swing_State>();
        wFeature.TransitionState(this, slash_swing_state);
        slash_swing_state.Initialize(wFeature, mouseSwipe);
    }

    public void Initialize(
        SlashAttackFeature _wFeature,
        Vector3 _mouseSwipe)
    {
        wFeature = _wFeature;
        mouseSwipe = _mouseSwipe;
    }

    public override void Behave()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration; //0-1

        DrawToPosition(drawProgress);
        DrawToRotation(drawProgress);

        //When done drawing
        if (drawProgress >= 1)
        {
            EndState();
        }
    }

    float CalculateTargetSlashAngle()
    {
        //Only half of possible angles need to be accounted for because they are functionally the same
        //If sign of x is negative, flip signs of x and y
        float signAdjustment = (mouseSwipe.x < 0) ? -1 : 1;
        Vector3 adjustedMouseSwipe = signAdjustment * mouseSwipe;
        float targetZAngle = Mathf.Rad2Deg * Mathf.Atan(adjustedMouseSwipe.y / adjustedMouseSwipe.x);
        targetZAngle -= 90.0f;
        return targetZAngle;
    }

    void DrawToPosition(float drawProgress)
    {
        //Draw sword in opposite direction as mouseSwipe
        Vector3 drawDir = new Vector3(-mouseSwipe.x, -mouseSwipe.y, 0);

        //Calculate target world position
        Vector3 localDelta = new Vector3(drawDir.x * drawDistance, drawDir.y * drawDistance, 0);
        Vector3 targetLocalPos = wFeature.weaponController.defaultPosition + localDelta;
        Transform skelyTransform = wFeature.weaponController.ownerSkely.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);

        //Lerp position
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(wFeature.weaponController.defaultPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);

        drawnLocalPosition = targetLocalPos;
    }

    void DrawToRotation(float drawProgress)
    {
        Vector3 drawDir = new Vector3(-mouseSwipe.x, mouseSwipe.y, 0);

        //Calculate target local rotation
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        float targetZAngle = CalculateTargetSlashAngle();

        Quaternion deltaRotation =
            Quaternion.Euler(
            drawDir.y * drawAngle,
            drawDir.x * drawAngle,
            targetZAngle
            );

        Quaternion targetRotation = zeroRotation * deltaRotation;

        //Lerp rotation
        Quaternion initialRotation = Quaternion.Euler(wFeature.weaponController.defaultRotation);
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, drawProgress);

        drawnLocalRotation = targetRotation;
    }
}
