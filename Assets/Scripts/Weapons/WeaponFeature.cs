using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

/*
>Goes on the weapon gameObject
>Intended to be inherited from

>Self-contained state machines of behavior that, once activated, control the behavior of the player
>When activated, disable other features
>When deactivated, reenable other features
*/

public abstract class WeaponFeature : MonoBehaviourPunCallbacks
{
    public WeaponController weaponController;

    #region  Feature Phases
    public enum EFeaturePhase
    {
        Active,     //In control of player behavior and still reading input
        Enabled,    //Reading input but not in control
        Disabled    //Totally muted
    }

    protected EFeaturePhase featurePhase = EFeaturePhase.Enabled;

    public EFeaturePhase GetPhase() { return featurePhase; }
    public void EnableFeature() { featurePhase = EFeaturePhase.Enabled; }
    public void DisableFeature() { featurePhase = EFeaturePhase.Disabled; }
    #endregion


    #region Feature States
    protected FeatureState activeState = null;
    protected FeatureState initialState = null;

    public void TransitionState(FeatureState oldState, FeatureState newState)
    {
        activeState = newState;
        if (oldState)
            Destroy(oldState);
        newState.BeginState();
    }
    #endregion



    void Start()
    {
        weaponController.collisionEvent += OnWeaponCollision;
    }

    protected virtual void Update()
    {
        if (featurePhase == EFeaturePhase.Disabled) { return; }
        if (!photonView.IsMine) { return; }

        if (activeState != null && featurePhase == EFeaturePhase.Active)
        {
            activeState.Behave();
        }
    }

    protected void Activate()
    {
        featurePhase = EFeaturePhase.Active;
        weaponController.DisableFeatures(this);
        SetInitialState();
        TransitionState(null, initialState);
    }

    public void Deactivate()
    {
        EnableFeature();
        weaponController.EnableFeatures(this);
    }

    protected abstract void SetInitialState();


    virtual public void OnWeaponCollision(Collider other) { return; }
}
