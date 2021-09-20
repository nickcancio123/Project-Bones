using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil_State : FeatureState
{
    SlashAttackFeature wFeature;
    float recoilDuration = 0;

    float startTime = 0;
    Vector3 startLocalPos;
    Quaternion startLocalRotation;
    
    protected override void CreateNextState() => nextState = gameObject.AddComponent<Reset_State>();
    
    public override void BeginState()
    {
        Initialize();
        
        startTime = Time.time;
        startLocalPos = transform.localPosition;
        startLocalRotation = transform.localRotation;
    }

    protected override void EndState() => TransitionState();

    public override void Behave()
    {
        float recoilProgress = (Time.time - startTime) / recoilDuration;

        transform.localPosition = Vector3.Lerp(startLocalPos, wFeature.drawnTransform.localPosition, recoilProgress);
        transform.localRotation = Quaternion.Lerp(startLocalRotation, wFeature.drawnTransform.localRotation, recoilProgress);

        if (recoilProgress >= 1)
        {
            EndState();
        }
    }

    void Initialize()
    {
        wFeature = GetComponent<SlashAttackFeature>();
        recoilDuration = wFeature.recoilDuration;
    }
    
}
