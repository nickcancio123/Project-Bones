using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

//This script goes on the player game object
public class CameraController : MonoBehaviourPunCallbacks
{
    [SerializeField] Vector3 followOffset;

    Camera mainCam;

    private void Start() => mainCam = Camera.main;

    void LateUpdate()
    {
        if (!photonView.IsMine) { return; }

        //Follow player by follow offset
        mainCam.transform.position = transform.position + followOffset;

        //Rotate camera yaw to player yaw
        Vector3 cameraEulers = mainCam.transform.rotation.eulerAngles;
        Vector3 playerEulers = transform.rotation.eulerAngles;
        mainCam.transform.rotation = Quaternion.Euler(cameraEulers.x, playerEulers.y, cameraEulers.z);
    }
}
