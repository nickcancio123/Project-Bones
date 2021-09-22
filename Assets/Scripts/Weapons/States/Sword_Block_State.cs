using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Block_State : FeatureState
{
    SwordBlockFeature wFeature;
    WeaponController wController;

    float mouseX = 0;
    float netMouseX = 0;

    protected override void CreateNextState() => nextState = gameObject.AddComponent<Reset_State>();

    public override void BeginState()
    {
        Initialize();
        wFeature.isBlocking = true;
    }

    protected override void EndState()
    {
        wFeature.isBlocking = false;
        wController.movementManager.canRotate = true;
        TransitionState();
    }

    public override void Behave()
    {
        ReadInput();
        Block();
        FaceNearestPlayer();
    }

    void Initialize()
    {
        wFeature = GetComponent<SwordBlockFeature>();
        wController = wFeature.weaponController;
    }
    
    void ReadInput()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX = 0;
            mouseX = Input.GetAxis("Mouse X");
            netMouseX += mouseX;
        }
        else
            EndState();
    }

    void Block()
    {
        bool canRotate = CanRotateBlock();

        if (canRotate)
        {
            //Sword tip follows mouse X input
            float rotationDirection = -1;
            //Clamp mouse for consistent experience
            float clampedMouseX = Mathf.Clamp(mouseX, -5, 5);

            //Rotate sword about player forward
            Vector3 rotationAxis = wController.ownerPlayer.transform.forward;
            float deltaRotation = rotationDirection * clampedMouseX * wFeature.blockRotationSpeed;
            transform.RotateAround(wFeature.swordMidPoint.position, rotationAxis, deltaRotation);
        }

        CalculateBlockAngle();
    }

    bool CanRotateBlock()
    {
        Transform skelyTransform = wController.ownerPlayer.transform;
        Vector3 swordForwardProjectedOntoYZPlane = Vector3.ProjectOnPlane(transform.forward, skelyTransform.forward);
        float swordAngle = Vector3.Angle(Vector3.up, swordForwardProjectedOntoYZPlane);

        //If already a valid angle
        if (swordAngle < 90)
        {
            return true;
        }

        //If out of bound but rotating back into bounds
        if ((int)Mathf.Sign(mouseX) != (int)Mathf.Sign(netMouseX))
        {
            return true;
        }

        return false;
    }

    void FaceNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject closestPlayer = null;
        float shortestDistanceToPlayer = 10000.0f;
        closestPlayer = GetClosestPlayer(players, out shortestDistanceToPlayer);

        if (!closestPlayer) { return; }

        //Rotate towards closest player
        Transform playerTransform = wController.ownerPlayer.transform;
        Vector3 directionToClosestPlayer = closestPlayer.transform.position - playerTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToClosestPlayer, Vector3.up);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, targetRotation, wFeature.blockLockOnSpeed);
    }

    GameObject GetClosestPlayer(GameObject[] players, out float shortestDistance)
    {
        GameObject closestPlayer = null;
        shortestDistance = 1000;

        Transform playerTransform = wController.ownerPlayer.transform;

        foreach (GameObject otherPlayer in players)
        {
            //Omit self
            if (otherPlayer.transform.root == playerTransform) { continue; }

            Renderer playerRenderer = otherPlayer.GetComponentInChildren<Renderer>();

            //Only check players within view
            if (!playerRenderer) { continue; }
            if (!playerRenderer.isVisible) { continue; }

            float distanceToPlayer = Vector3.Distance(playerTransform.position, otherPlayer.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                closestPlayer = otherPlayer;
                shortestDistance = distanceToPlayer;
            }
        }
        return closestPlayer;
    }

    void CalculateBlockAngle()
    {
        Transform playerTransform = wController.ownerPlayer.transform;
        Vector3 swordForwardProjectedOntoYZPlane = 
            Vector3.ProjectOnPlane(transform.forward, playerTransform.forward);

        float angleSign = (transform.localPosition.x > 0) ? -1 : 1;
        wFeature.blockAngle = angleSign * Vector3.Angle(Vector3.up, swordForwardProjectedOntoYZPlane);
    }
}
