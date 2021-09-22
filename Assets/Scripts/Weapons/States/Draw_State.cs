using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Draw_State : FeatureState
{
    protected WeaponController wController;
    protected Vector3 targetLocalPos = Vector3.zero;
    protected Quaternion targetLocalRotation = Quaternion.identity;
    protected float drawDuration = 0;

    float drawStartTime = 0;
    Vector3 startingLocalPos = Vector3.zero;
    Quaternion startingLocalRotation = Quaternion.identity;
    
    protected override void CreateNextState() => nextState = gameObject.AddComponent<Reset_State>();

    public override void BeginState()
    {
        Initialize();

        wController = baseFeature.weaponController;
        wController.DisableAnimator();

        drawStartTime = Time.time;
        startingLocalPos = transform.localPosition;
        startingLocalRotation = transform.localRotation;
        SetTargetLocalPosition();
        SetTargetLocalRotation();
        SetDrawDuration();
    }

    public override void Behave()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration; //0-1
        
        DrawToPosition(drawProgress);
        DrawToRotation(drawProgress);
        
        if (drawProgress >= 1)
            EndState();
    }

    protected override void EndState() => TransitionState();

    protected virtual void Initialize() {}

    protected abstract void SetTargetLocalPosition();
    protected abstract void SetTargetLocalRotation();
    protected abstract void SetDrawDuration();
    
    void DrawToPosition(float drawProgress)
    {
        Transform playerTransform = wController.ownerPlayer.transform;
        Vector3 startingPos = playerTransform.position + playerTransform.TransformDirection(startingLocalPos);
        Vector3 targetWorldPos = playerTransform.position + playerTransform.TransformDirection(targetLocalPos);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);
    }
    
    void DrawToRotation(float drawProgress)
    {
        transform.localRotation = Quaternion.Lerp(startingLocalRotation, targetLocalRotation, drawProgress);
    }
}
