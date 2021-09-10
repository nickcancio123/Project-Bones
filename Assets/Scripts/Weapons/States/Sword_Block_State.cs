using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Block_State : FeatureState
{
    SwordBlockFeature wFeature;
    WeaponController wController;

    float mouseX = 0;
    float netMouseX = 0;

    void Awake()
    {
        wFeature = GetComponent<SwordBlockFeature>();
        wController = wFeature.weaponController;
    }

    public override void BeginState()
    {
        wFeature.isBlocking = true;
    }

    protected override void EndState()
    {
        wFeature.isBlocking = false;
        wController.skelyMovement.canRotate = true;

        Reset_State reset_state = gameObject.AddComponent<Reset_State>();
        wFeature.TransitionState(this, reset_state);
    }

    public override void Behave()
    {
        ReadInput();
        Block();
        FaceNearestPlayer();
    }

    void ReadInput()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX = 0;
            mouseX = Input.GetAxis("Mouse X");
            netMouseX += mouseX;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            EndState();
        }
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
            Vector3 rotationAxis = wController.ownerSkely.transform.forward;
            float deltaRotation = rotationDirection * clampedMouseX * wFeature.blockRotationSpeed;
            transform.RotateAround(wFeature.swordMidPoint.position, rotationAxis, deltaRotation);
        }

        CalculateBlockAngle();
    }

    bool CanRotateBlock()
    {
        Transform skelyTransform = wController.ownerSkely.transform;
        Vector3 swordForwardProjectedOntoYZPlane = Vector3.ProjectOnPlane(transform.forward, skelyTransform.forward);
        float swordAngle = Vector3.Angle(Vector3.up, swordForwardProjectedOntoYZPlane);

        //If already a valid angle
        if (swordAngle < 90)
        {
            return true;
        }

        //If out of bound but rotating back into bounds
        if (Mathf.Sign(mouseX) != Mathf.Sign(netMouseX))
        {
            return true;
        }

        return false;
    }

    void FaceNearestPlayer()
    {
        //Get all players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //Get closest player
        GameObject closestPlayer = null;
        float shortestDistanceToPlayer = 10000.0f;

        closestPlayer = GetClosestPlayer(players, out shortestDistanceToPlayer);

        //If no valid player, return
        if (!closestPlayer) { return; }

        //Rotate towards closest player
        Transform skelyTransform = wController.ownerSkely.transform;
        Vector3 directionToClosestPlayer = closestPlayer.transform.position - skelyTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToClosestPlayer, Vector3.up);
        skelyTransform.rotation = Quaternion.Lerp(skelyTransform.rotation, targetRotation, wFeature.blockLockOnSpeed);
    }

    GameObject GetClosestPlayer(GameObject[] players, out float shortestDistance)
    {
        GameObject closestPlayer = null;
        shortestDistance = 1000;

        Transform skelyTransform = wController.ownerSkely.transform;

        foreach (GameObject otherPlayer in players)
        {
            //Omit self
            if (otherPlayer.transform.root == skelyTransform) { continue; }

            Renderer renderer = otherPlayer.GetComponentInChildren<Renderer>();

            //Only check players within view
            if (!renderer) { continue; }
            if (!renderer.isVisible) { continue; }

            float distanceToPlayer = Vector3.Distance(skelyTransform.position, otherPlayer.transform.position);
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
        Transform skelyTransform = wController.ownerSkely.transform;
        Vector3 swordForwardProjectedOntoYZPlane = Vector3.ProjectOnPlane(transform.forward, skelyTransform.forward);

        float angleSign = (transform.localPosition.x > 0) ? -1 : 1;
        wFeature.blockAngle = angleSign * Vector3.Angle(Vector3.up, swordForwardProjectedOntoYZPlane);
    }
}
