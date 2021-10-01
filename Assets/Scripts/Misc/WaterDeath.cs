using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterDeath : MonoBehaviour
{
    [SerializeField] float killDelay = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            StartCoroutine(DelayKillPlayer(other.gameObject));
    }

    IEnumerator DelayKillPlayer(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        if (health)
            health.TakeDamage(100);
        
        yield return new WaitForSeconds(killDelay);
    }
}
