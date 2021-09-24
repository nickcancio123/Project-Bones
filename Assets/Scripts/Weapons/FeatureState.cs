using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeatureState : MonoBehaviour
{
    protected WeaponFeature baseFeature;
    
    protected FeatureState nextState;
    protected abstract void CreateNextState();

    private void Awake()
    {
        Component comp = GetComponent<WeaponController>()?.GetActiveFeatureAsComponent();
        baseFeature = (WeaponFeature) comp;
    }

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
