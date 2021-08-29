using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject skelyPrefab;

    void Start()
    {
        if (skelyPrefab)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-5, 5), 5, Random.Range(-5, 5));
            GameObject skely = PhotonNetwork.Instantiate(this.skelyPrefab.name, spawnPoint, Quaternion.identity, 0);

            Health skelyHealth = skely.GetComponent<Health>();
            if (skelyHealth)
                skelyHealth.deathEvent += OnPlayerDeath;
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void OnPlayerDeath()
    {
        LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
