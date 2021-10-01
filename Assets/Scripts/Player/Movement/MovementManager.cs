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
        
        Move();
        Look();
    }

    void Move()
    {
        if (!canMove) { return; }

        Vector3 movement = Vector3.zero;

        //Find first exclusive modifier
        bool containsExclusiveMod = false;
        MovementModifier exclusiveMod = null;
        foreach (MovementModifier modifier in modifiers)
        {
            if (modifier.IsExclusive())
            {
                containsExclusiveMod = true;
                exclusiveMod = modifier;
                break;
            }
        }

        foreach (MovementModifier modifier in modifiers)
        {
            if (containsExclusiveMod)
            {
                //Only affect movement if is the exclusive mod or is compatible with the exclusive mod
                if (modifier == exclusiveMod || exclusiveMod.compatibleModTypes.Contains(modifier.GetModType()))
                {
                    movement += modifier.GetValue();
                }
            }
            else
            {
                //If no exclusive mod, every mod affects movement
                movement += modifier.GetValue();
            }
        }

        characterController.Move(movement * Time.deltaTime);
    }

    void Look()
    {
        if (!canRotate) { return; }

        float deltaPitch = -Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        float currentPitch = Vector3.SignedAngle(transform.forward, mainCam.transform.forward, transform.right);
        float pitch = currentPitch + deltaPitch;
        pitch = Mathf.Clamp(pitch, -pitchLimit, pitchLimit);

        float deltaYaw = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
        float yaw = transform.rotation.eulerAngles.y + deltaYaw;

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        mainCam.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    public float GetSpeed() { return characterController.velocity.magnitude; }
}
