using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (playerPrefab)
        {
            //Spawn player
            int spawnSide = Random.Range(0, 2);
            Vector3 spawnPoint = new Vector3( Mathf.Pow(-1, spawnSide) * Random.Range(10, 20), 5, Random.Range(-20, 20));
            
            
            GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint, Quaternion.identity, 0);

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth)
                playerHealth.deathEvent += OnPlayerDeath;
            
            //Spawn weapon
            GameObject weapon = PhotonNetwork.Instantiate(PlayerWeaponInfo.selectedWeaponPrefab.name, Vector3.zero, Quaternion.identity);
            weapon.transform.SetParent(player.transform, false);
            IWeapon iWeapon = weapon.GetComponent<IWeapon>();
            if (iWeapon)
                iWeapon.SyncRefs(PhotonView.Get(player).ViewID);
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
