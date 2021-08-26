using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

/*
>Goes on player game object
*/
public class IHealth : MonoBehaviourPunCallbacks
{
    [SerializeField] Slider healthBar;

    [SerializeField] float maxHealth = 100;
    float currentHealth = 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            UpdateHealthBar();
            PollDeath();
        }
    }

    void UpdateHealthBar()
    {
        if (!healthBar) { return; }

        healthBar.value = Mathf.Clamp(currentHealth / maxHealth, 0, 1);
    }

    public void TakeWeaponDamage(float maxDamageAmount, int attackerID)
    {
        photonView.RPC("RPC_TakeWeaponDamage", RpcTarget.All, maxDamageAmount, attackerID);
    }

    [PunRPC]
    void RPC_TakeWeaponDamage(float maxDamageAmount, int attackerID)
    {
        if (!photonView.IsMine) { return; }

        //Get block feature
        GameObject weapon = gameObject.GetComponentInChildren<IWeapon>()?.gameObject;
        if (!weapon) { return; }
        BlockFeature blockFeature = weapon.GetComponent<BlockFeature>();

        //Apply damage
        float damageTaken = maxDamageAmount;

        if (blockFeature)
        {
            damageTaken = blockFeature.BlockAttack(maxDamageAmount, attackerID);
        }

        currentHealth -= damageTaken;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    [PunRPC]
    void RPC_TakeDamage(float damageAmount)
    {
        if (!photonView.IsMine) { return; }

        //Apply damage
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void PollDeath()
    {
        if (currentHealth <= 0)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().LeaveRoom();
        }
    }
}
