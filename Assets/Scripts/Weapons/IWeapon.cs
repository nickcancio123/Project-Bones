using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
>Goes on the weapon game object
>Identifies the weapon as a weapon
>Holds info about the weapon
*/

public class IWeapon : MonoBehaviourPunCallbacks
{
    [HideInInspector] public GameObject ownerPlayer;
    public Transform leftGrip;
    public Transform rightGrip;

    
    public void SyncRefs(int playerViewID) => photonView.RPC("RPC_SyncRefs", RpcTarget.AllBuffered, playerViewID);
    
    [PunRPC]    //Needed because refs between weapon and player are not synced automatically
    void RPC_SyncRefs(int playerViewID)
    {
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        ownerPlayer = player;
        transform.SetParent(player.transform, false);
    }
}
