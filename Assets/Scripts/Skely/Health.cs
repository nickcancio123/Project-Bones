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

    public void TakeWeaponDamage(float maxDamageAmount, int attackerID)
    {
        photonView.RPC("RPC_TakeWeaponDamage", RpcTarget.All, maxDamageAmount, attackerID);
    }

    public void TakeDamage(float damageAmount)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damageAmount);
    }
    #endregion



    #region RPCs
    [PunRPC]
    void RPC_TakeWeaponDamage(float maxDamageAmount, int attackerID)
    {
        if (!photonView.IsMine) { return; }

        GameObject weapon = gameObject.GetComponentInChildren<IWeapon>()?.gameObject;
        BlockFeature blockFeature = weapon?.GetComponent<BlockFeature>();

        float damageTaken = maxDamageAmount;

        if (blockFeature)
        {
            damageTaken = blockFeature.BlockAttack(maxDamageAmount, attackerID);
        }

        currentHealth = Mathf.Clamp(currentHealth - damageTaken, 0, maxHealth);

        if (currentHealth <= 0)
            deathEvent();
    }

    [PunRPC]
    void RPC_TakeDamage(float damageAmount)
    {
        if (!photonView.IsMine) { return; }

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        if (currentHealth <= 0)
            deathEvent();
    }
    #endregion    
}
