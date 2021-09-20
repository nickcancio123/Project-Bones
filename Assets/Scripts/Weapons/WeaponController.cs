using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/*
>Goes on the weapon gameObject
>Manages the features on a weapon
>Manages weapon animation
*/
public class WeaponController : MonoBehaviourPunCallbacks
{
    [SerializeField] protected Animator weaponAnimator;

    public AudioSource weaponAudioSource;
    public Vector3 defaultPosition;
    public Vector3 defaultRotation;

    [HideInInspector] public GameObject ownerPlayer;
    [HideInInspector] public MovementManager movementManager;


    protected void Start()
    {
        CacheReferences();

        transform.localPosition = defaultPosition;
        transform.localRotation = Quaternion.Euler(defaultRotation);
    }

    void CacheReferences()
    {
        ownerPlayer = gameObject.transform.parent.gameObject;
        movementManager = ownerPlayer.GetComponent<MovementManager>();
        weaponAnimator = gameObject.GetComponent<Animator>();
    }


    #region Feature Phase Interface
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

    public Component GetActiveFeatureAsComponent()  //Can be casted to preferred type by caller
    {
        WeaponFeature[] features = GetComponents<WeaponFeature>();
        foreach (WeaponFeature feature in features)
        {
            if (feature.GetPhase() == WeaponFeature.EFeaturePhase.Active)
                return (Component) feature;
        }
        return null;
    }
    #endregion


    #region Collision
    public delegate void collisionEventCallback(Collider other);
    public event collisionEventCallback collisionEvent;

    public void OnWeaponCollision(Collider other)
    {
        if (collisionEvent != null)
            collisionEvent(other);
    }
    #endregion


    #region Weapon Animator
    public void DisableAnimator()
    {
        photonView.RPC("RPC_DisableWeaponAnimator", RpcTarget.All);
    }

    public void EnableAnimator()
    {
        photonView.RPC("RPC_EnableWeaponAnimator", RpcTarget.All);
    }

    [PunRPC]
    protected void RPC_DisableWeaponAnimator()
    {
        weaponAnimator = gameObject.GetComponent<Animator>();
        if (weaponAnimator)
            weaponAnimator.enabled = false;
    }

    [PunRPC]
    protected void RPC_EnableWeaponAnimator()
    {
        weaponAnimator = gameObject.GetComponent<Animator>();
        if (weaponAnimator)
            weaponAnimator.enabled = true;
    }
    #endregion
}
