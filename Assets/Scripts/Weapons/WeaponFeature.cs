using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
>Goes on the weapon gameObject
>Intended to be inherited from

>Self-contained states of behavior that, once activated, control the behavior of the player
>When activated, disable other features
>When deactivated, reenable other features
*/

public abstract class WeaponFeature : MonoBehaviour
{
    [SerializeField] protected WeaponController weaponController;

    public enum EFeatureState
    {
        Active,     //In control of player behavior and still reading input
        Enabled,    //Reading input but not in control
        Disabled    //Totally muted
    }

    protected EFeatureState featureState = EFeatureState.Enabled;

    //Public state interface
    public EFeatureState GetState() { return featureState; }
    public void EnableFeature() { featureState = EFeatureState.Enabled; }
    public void DisableFeature() { featureState = EFeatureState.Disabled; }


    //Self-activation
    protected void Activate()
    {
        featureState = EFeatureState.Active;
        weaponController.DisableFeatures(this);
    }

    //Self-deactivation
    protected void Deactivate()
    {
        EnableFeature();
        weaponController.EnableFeatures(this);
    }


    protected abstract void ReadInput();
}
