using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MovementManager : MonoBehaviourPunCallbacks
{
    public CharacterController characterController;

    [SerializeField] float lookSpeed = 10;
    [SerializeField] float pitchLimit = 70;

    [HideInInspector] public GameObject ownerPlayer;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canRotate = true;
    [HideInInspector] public bool isRunning = false;

    List<MovementModifier> modifiers = new List<MovementModifier>();

    float pitch = 0;
    float yaw = 0;
    Camera mainCam;


    public void AddModifier(MovementModifier modifier) => modifiers.Add(modifier);
    public void RemoveModifier(MovementModifier modifier) => modifiers.Remove(modifier);

    void Start()
    {
        ownerPlayer = this.gameObject;
        mainCam = Camera.main;
    }

    void Update()
    {
        if (!photonView.IsMine) { return; }

        print(characterController.velocity.magnitude);

        Move();
        Look();
    }

    void Move()
    {
        if (!canMove) { return; }

        Vector3 movement = Vector3.zero;

        foreach (MovementModifier modifier in modifiers)
        {
            movement += modifier.GetValue();
        }

        characterController.Move(movement * Time.deltaTime);
    }

    void Look()
    {
        if (!canRotate) { return; }

        pitch += -Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -pitchLimit, pitchLimit);
        yaw += Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        mainCam.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    public float GetSpeed() { return characterController.velocity.magnitude; }
}
