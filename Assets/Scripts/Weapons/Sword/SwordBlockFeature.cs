using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

/*
>Block feature broken into 3 phases (not including idle phase)
    -Draw: move sword to block default position
    -Block: rotate sword based on mouse X input
    -Reset: move sword back to default state
*/

public class SwordBlockFeature : BlockFeature
{
    #region General
    enum EBlockPhase
    {
        Idle,
        Draw,
        Block,
        Reset
    }

    EBlockPhase blockPhase = EBlockPhase.Idle;

    [SerializeField] Vector3 defaultBlockPosition;
    [SerializeField] Vector3 defaultBlockRotation;
    [SerializeField] Transform swordMidPoint;
    [SerializeField] float angleToHitCuttoff = 1;
    [SerializeField] float maxAngleToFaceAttackerToBeHit = 1;

    float blockAngle = 0;
    #endregion



    #region Draw Variables
    [Header("Draw")]
    [SerializeField] float drawDuration = 1;

    float drawStartTime = 0;
    #endregion



    #region Block Variables
    [Header("Block")]
    [SerializeField] float blockRotationSpeed = 1;
    [SerializeField] float blockLockOnSpeed = 1;

    float mouseX = 0;
    float netMouseX = 0;

    Vector3 finalBlockLocalPos = Vector3.zero;
    Quaternion finalBlockLocalRotation = Quaternion.identity;
    #endregion



    #region Reset Variables
    [Header("Reset")]
    [SerializeField] float resetDuration = 1;

    float resetStartTime = 0;
    #endregion


    void Update()
    {
        if (featureState == EFeatureState.Disabled) { return; }
        if (!photonView.IsMine) { return; }

        Behavior();
    }


    void Behavior()
    {
        //Always read input because input informs blocking
        ReadInput();

        switch (blockPhase)
        {
            case EBlockPhase.Idle:
                //No special behavior
                break;

            case EBlockPhase.Draw:
                Draw();
                break;

            case EBlockPhase.Block:
                Block();
                FaceNearestPlayer();
                break;

            case EBlockPhase.Reset:
                Reset();
                break;
        }
    }

    void ReadInput()
    {
        //Start blocking
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            netMouseX = 0;
            BeginDraw();
            //***ACTIVATING FEATURE***
            Activate();
        }

        //Read block input
        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX = 0;
            mouseX = Input.GetAxis("Mouse X");
            netMouseX += mouseX;
        }

        //End block: begin resetting
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            BeginReset();
        }
    }


    #region Draw Methods
    void BeginDraw()
    {
        weaponController.DisableAnimator();
        weaponController.playerMovement.canRotate = false;

        blockPhase = EBlockPhase.Draw;
        drawStartTime = Time.time;
    }

    void Draw()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration;

        //Draw to position
        Transform skelyTransform = weaponController.ownerPlayer.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(defaultBlockPosition);
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(weaponController.defaultPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);

        //Draw to rotation
        Quaternion initialRotation = Quaternion.Euler(weaponController.defaultRotation);
        Quaternion targetRotation = Quaternion.Euler(defaultBlockRotation);
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, drawProgress);

        //Once sword is drawn
        if (drawProgress > 1)
        {
            BeginBlock();
        }
    }
    #endregion


    #region Block Methods
    void BeginBlock()
    {
        blockPhase = EBlockPhase.Block;
        isBlocking = true;
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
            Vector3 rotationAxis = weaponController.ownerPlayer.transform.forward;
            float deltaRotation = rotationDirection * clampedMouseX * blockRotationSpeed;
            transform.RotateAround(swordMidPoint.position, rotationAxis, deltaRotation);
        }

        CalculateBlockAngle();
    }

    bool CanRotateBlock()
    {
        Transform skelyTransform = weaponController.ownerPlayer.transform;
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
        Transform skelyTransform = weaponController.ownerPlayer.transform;
        Vector3 directionToClosestPlayer = closestPlayer.transform.position - skelyTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToClosestPlayer, Vector3.up);
        skelyTransform.rotation = Quaternion.Lerp(skelyTransform.rotation, targetRotation, blockLockOnSpeed);
    }

    GameObject GetClosestPlayer(GameObject[] players, out float shortestDistance)
    {
        GameObject closestPlayer = null;
        shortestDistance = 1000;

        Transform skelyTransform = weaponController.ownerPlayer.transform;

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
        Transform skelyTransform = weaponController.ownerPlayer.transform;
        Vector3 swordForwardProjectedOntoYZPlane = Vector3.ProjectOnPlane(transform.forward, skelyTransform.forward);

        float angleSign = (transform.localPosition.x > 0) ? -1 : 1;
        blockAngle = angleSign * Vector3.Angle(Vector3.up, swordForwardProjectedOntoYZPlane);
    }
    #endregion


    #region Reset Methods
    void BeginReset()
    {
        blockPhase = EBlockPhase.Reset;
        isBlocking = false;
        resetStartTime = Time.time;

        //Unlock rotation
        weaponController.playerMovement.canRotate = true;

        finalBlockLocalPos = transform.localPosition;
        finalBlockLocalRotation = transform.localRotation;
    }

    void Reset()
    {
        float resetProgress = (Time.time - resetStartTime) / resetDuration;

        Transform skelyTransform = weaponController.ownerPlayer.transform;

        //Reset to default position
        transform.localPosition = Vector3.Lerp(finalBlockLocalPos, weaponController.defaultPosition, resetProgress);

        //Reset to default rotation
        transform.localRotation =
            Quaternion.Lerp(finalBlockLocalRotation, Quaternion.Euler(weaponController.defaultRotation), resetProgress);

        //Reset system
        if (resetProgress > 1)
        {
            blockPhase = EBlockPhase.Idle;

            weaponController.EnableAnimator();

            //*****DEACTIVATE FEATURE*****
            Deactivate();

        }
    }
    #endregion



    #region Block Functionality
    public override float BlockAttack(float maxDamageAmount, int attackerID)
    {
        if (!isBlocking) { return maxDamageAmount; }

        float damageTaken = maxDamageAmount;

        //Get attack feature from attacker ID
        PhotonView attackerPV = PhotonView.Find(attackerID);
        GameObject weapon = attackerPV?.gameObject.GetComponentInChildren<IWeapon>()?.gameObject;
        AttackFeature attackFeature = weapon?.GetComponentInChildren<AttackFeature>();

        //Get attacker position from attackerID
        Vector3 attackerPosition = attackerPV.gameObject.transform.position;

        switch (attackFeature.attackType)
        {
            case AttackType.Slash:
                {
                    damageTaken = BlockSwipeAttack(maxDamageAmount, attackFeature, attackerPosition);
                    break;
                }

            case AttackType.Jab:
                {
                    break;
                }
        }

        return damageTaken;
    }

    float BlockSwipeAttack(float maxDamageAmount, AttackFeature attackFeature, Vector3 attackerPosition)
    {
        //If not facing enemy, take full damage
        Transform skelyTransform = weaponController.ownerPlayer.transform;

        Vector3 directionToAttacker = attackerPosition - skelyTransform.position;
        float deltaAngleToFaceAttacker = Vector3.Angle(skelyTransform.forward, directionToAttacker);

        if (deltaAngleToFaceAttacker > maxAngleToFaceAttackerToBeHit)
        {
            //Your back is to the attacker, you did not block the attack
            return maxDamageAmount;
        }

        //Negated to make local space
        float attackAngle = -attackFeature.attackAngle;

        float incidentAngle = Mathf.Abs(blockAngle - attackAngle);

        if (incidentAngle > 90)
        {
            incidentAngle = Mathf.Abs(180 - incidentAngle);
        }

        bool gotHit = (incidentAngle < angleToHitCuttoff) ? true : false;

        if (gotHit)
        {
            return maxDamageAmount;
        }
        else
        {
            PlayBlockEffects();
            return 0;
        }
    }
    #endregion
}
