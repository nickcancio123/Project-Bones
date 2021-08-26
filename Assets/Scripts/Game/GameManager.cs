using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject skelyPrefab;

    void Start()
    {
        if (skelyPrefab)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-5, 5), 5, Random.Range(-5, 5));
            PhotonNetwork.Instantiate(this.skelyPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.LeaveRoom();
    }
}
