using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            StartCoroutine(KillPlayerDelay(other.gameObject));
    }
    
    IEnumerator KillPlayerDelay(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        if (health)
            health.TakeDamage(100);
        yield return new WaitForSeconds(0.7f);
    }
}
