using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class CameraController : MonoBehaviourPunCallbacks
{
    [SerializeField] Vector3 followOffset;

    void LateUpdate()
    {
        if (!photonView.IsMine) { return; }
        //Move with player
        Camera.main.transform.position = this.transform.position + followOffset;

        //Rotate yaw to match player
        Vector3 cameraEulerAngles = Camera.main.transform.rotation.eulerAngles;
        Vector3 playerEulerAngles = this.transform.rotation.eulerAngles;
        Camera.main.transform.rotation = Quaternion.Euler(cameraEulerAngles.x, playerEulerAngles.y, cameraEulerAngles.z);
    }
}
