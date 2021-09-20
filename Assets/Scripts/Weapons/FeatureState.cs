using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeatureState : MonoBehaviour
{
    WeaponFeature baseFeature;

    protected FeatureState nextState;
    protected abstract void CreateNextState();

    private void Awake() => baseFeature = GetComponent<WeaponController>()?.GetActiveFeature();

    public abstract void BeginState();
    public abstract void Behave();
    protected abstract void EndState();

    protected void TransitionState()
    {
        CreateNextState();
        if (baseFeature)
            baseFeature.TransitionState(this, nextState);
    }
}
