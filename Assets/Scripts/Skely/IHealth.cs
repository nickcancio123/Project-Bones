using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

/*
>Goes on player game object
*/
public class IHealth : MonoBehaviourPunCallbacks
{
    [SerializeField] float maxHealth = 100;
    float currentHealth = 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

    }

    [PunRPC]
    void RPC_TakeDamage(AttackFeature attackScript, float maxDamageAmount, GameObject attacker)
    {
        if (photonView.IsMine)
        {
            //Get block feature
            GameObject weapon = gameObject.GetComponentInChildren<IWeapon>()?.gameObject;
            if (!weapon) { return; }
            BlockFeature blockFeature = weapon.GetComponent<BlockFeature>();

            //Apply damage
            float damageTaken = maxDamageAmount;

            if (blockFeature)
            {
                damageTaken = blockFeature.BlockAttack(attackScript, maxDamageAmount, attacker);
            }

            currentHealth -= damageTaken;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
    }
}
