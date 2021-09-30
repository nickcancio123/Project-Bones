using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

/*
>Goes on player game object
*/
public class Health : MonoBehaviourPunCallbacks
{
    [SerializeField] Slider healthBar;

    [SerializeField] float maxHealth = 100;
    float currentHealth = 0;

    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] AudioClip damageAudioClip;

    public event Action deathEvent;


    void Start()
    {
        currentHealth = maxHealth;
    }


    #region Update Loop
    void Update()
    {
        if (!photonView.IsMine) { return; }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (!healthBar) { return; }

        healthBar.value = Mathf.Clamp(currentHealth / maxHealth, 0, 1);
    }
    #endregion



    #region Public Interface
    float GetCurrentHealth() { return currentHealth; }
    float GetMaxHealth() { return maxHealth; }

    public void TakeWeaponDamage(float maxDamageAmount, int attackerID, bool isBlockeable)
    {
        photonView.RPC("RPC_TakeWeaponDamage", RpcTarget.All, maxDamageAmount, attackerID, isBlockeable);
    }

    public void TakeDamage(float damageAmount)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damageAmount);
    }
    #endregion



    #region RPCs
    [PunRPC]
    void RPC_TakeWeaponDamage(float maxDamageAmount, int attackerID, bool unblockable)
    {
        if (!photonView.IsMine) { return; }

        float damageTaken = maxDamageAmount;

        //Process a block
        if (!unblockable)
        {
            GameObject weapon = gameObject.GetComponentInChildren<IWeapon>()?.gameObject;
            BlockFeature blockFeature = weapon?.GetComponent<BlockFeature>();
            if (blockFeature)
                damageTaken = blockFeature.BlockAttack(maxDamageAmount, attackerID);
        }
        
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damageTaken);
    }

    [PunRPC]
    void RPC_TakeDamage(float damageTaken)
    {
        if (!photonView.IsMine) { return; }

        currentHealth = Mathf.Clamp(currentHealth - damageTaken, 0, maxHealth);

        if (damageTaken > 0)
        {
            photonView.RPC("RPC_PlayDamageEffects", RpcTarget.All);
        }

        if (currentHealth <= 0)
            deathEvent();
    }

    [PunRPC]
    void RPC_PlayDamageEffects()
    {
        playerAudioSource.PlayOneShot(damageAudioClip);
    }
    #endregion    
}
