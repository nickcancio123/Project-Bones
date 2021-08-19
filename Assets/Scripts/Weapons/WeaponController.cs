using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
>Goes on the weapon gameObject
>Manages the features on a weapon
>Manages weapon animation
*/
public class WeaponController : MonoBehaviour
{
    //Can be accessed before runtime because ref is in prefab
    [SerializeField] protected Animator weaponAnimator;

    //Can't be accessed before runtime because weapon is assigned to skely at runtime
    protected GameObject ownerSkely;
    protected SkelyMovementController skelyMovement;

    protected void Start()
    {
        CacheReferences();
    }

    void CacheReferences()
    {
        ownerSkely = gameObject.transform.parent.gameObject;

        if (!ownerSkely)
        {
            print("No owner skely ref");
            return;
        }

        skelyMovement = ownerSkely.GetComponent<SkelyMovementController>();

        if (!skelyMovement)
        {
            print("No skely movement ref");
            return;
        }
    }

    #region Feature State Interface
    public void DisableFeatures(WeaponFeature callingFeature)
    {
        WeaponFeature[] allMyFeatures = GetComponentsInChildren<WeaponFeature>(true);
        foreach (WeaponFeature feature in allMyFeatures)
        {
            //Don't affect the feature that called this function
            if (feature != callingFeature)
                feature.DisableFeature();
        }
    }

    public void EnableFeatures(WeaponFeature callingFeature)
    {
        WeaponFeature[] allMyFeatures = GetComponentsInChildren<WeaponFeature>(true);
        foreach (WeaponFeature feature in allMyFeatures)
        {
            //Don't affect the feature that called this function
            if (feature != callingFeature)
                feature.EnableFeature();
        }
    }
    #endregion
}
