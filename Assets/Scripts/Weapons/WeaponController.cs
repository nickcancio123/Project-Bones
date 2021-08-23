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
    [SerializeField] protected Animator weaponAnimator;
    public Vector3 defaultPosition;
    public Vector3 defaultRotation;

    [HideInInspector] public GameObject ownerSkely;
    [HideInInspector] public SkelyMovementController skelyMovement;


    protected void Start()
    {
        CacheReferences();

        transform.localPosition = defaultPosition;
        transform.localRotation = Quaternion.Euler(defaultRotation);
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

    public void DisableAnimator()
    {
        weaponAnimator.enabled = false;
    }

    public void EnableAnimator()
    {
        weaponAnimator.enabled = true;
    }
    #endregion
}
