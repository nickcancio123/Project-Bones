using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_Reset_State : FeatureState
{
    SlashAttackFeature wFeature;

    float resetDuration = 0;
    float resetStartTime = 0;

    Vector3 startLocalPos;
    Quaternion startLocalRotation;

    void Awake()
    {
        wFeature = GetComponent<SlashAttackFeature>();
        resetDuration = wFeature.resetDuration;
    }

    public override void BeginState()
    {
        print("reset");
        resetStartTime = Time.time;
        startLocalPos = transform.localPosition;
        startLocalRotation = transform.localRotation;
    }

    protected override void EndState()
    {
        wFeature.weaponController.EnableAnimator();
        //*****DEACTIVATE FEATURE*****
        wFeature.Deactivate();

        // FeatureState slash_draw_state = gameObject.AddComponent<Slash_Draw_State>();
        // wFeature.TransitionState(this, slash_draw_state);
    }

    public override void Behave()
    {
        float resetProgress = (Time.time - resetStartTime) / resetDuration;

        transform.localPosition = Vector3.Lerp(startLocalPos, wFeature.weaponController.defaultPosition, resetProgress);
        transform.localRotation =
            Quaternion.Lerp(startLocalRotation, Quaternion.Euler(wFeature.weaponController.defaultRotation), resetProgress);

        //Reset system 
        if (resetProgress > 1)
        {
            EndState();
        }
    }
}
