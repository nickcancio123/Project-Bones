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

    void Awake()
    {
        wFeature = GetComponent<SlashAttackFeature>();
        recoilDuration = wFeature.recoilDuration;
    }

    public override void BeginState()
    {
        print("Recoil");
        startTime = Time.time;
        startLocalPos = transform.localPosition;
        startLocalRotation = transform.localRotation;
    }

    protected override void EndState()
    {
        Reset_State reset_state = gameObject.AddComponent<Reset_State>();
        wFeature.TransitionState(this, reset_state);
    }

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
}
