using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        if (playerPrefab)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-5, 5), 5, Random.Range(-5, 5));
            GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint, Quaternion.identity, 0);

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth)
                playerHealth.deathEvent += OnPlayerDeath;
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
