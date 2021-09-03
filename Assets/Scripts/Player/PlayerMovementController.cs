using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerMovementController : MonoBehaviourPunCallbacks
{
    #region Editor Interface
    [SerializeField] CharacterController characterController;

    [SerializeField] float walkingSpeed = 7.5f;
    [SerializeField] float runningSpeed = 11.5f;
    [SerializeField] float jumpSpeed = 8.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float lookSpeed = 2.0f;
    [SerializeField] float lookXLimit = 45.0f;
    #endregion


    #region Cache
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float rotationY = 0;
    float movementDirectionY = 0;
    Vector3 lastFramePosition = Vector3.zero;
    #endregion


    #region Public Interface
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canRotate = true;
    [HideInInspector] public bool isRunning = false;

    [HideInInspector] public float velocity = 0.0f;
    #endregion

    void Start()
    {
        lastFramePosition = transform.position;
    }

    void Update()
    {
        if (!photonView.IsMine) { return; }

        HandleLocomotion();
        HandleRotation();
        HandleGravity();

        CalculateVelocity();
    }

    #region Movement Update Loop
    void HandleRotation()
    {
        if (canRotate)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY += Input.GetAxis("Mouse X") * lookSpeed;

            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

            //Rotate whole character for yaw
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleLocomotion()
    {
        //Gets world vectors from local forward and right vectors
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        //Jumping
        HandleJump();

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
    }

    void HandleGravity()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }

    #endregion


    void CalculateVelocity()
    {
        velocity = Vector3.Magnitude(transform.position - lastFramePosition) / Time.deltaTime;
        lastFramePosition = transform.position;
    }
}
