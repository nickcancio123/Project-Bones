using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
>Blade swipe direction should be in local Y-plane
*/

public class SwordAttackFeature : WeaponFeature
{
    #region General
    [SerializeField] TrailRenderer swipeTrail;

    enum EAttackPhase
    {
        Reading,
        Draw,
        Swipe,
        Reset
    }

    EAttackPhase attackPhase = EAttackPhase.Reading;

    Vector3 mouseSwipe = Vector3.zero;
    #endregion


    #region Draw Phase Variables
    [Header("Draw")]
    [SerializeField] float drawDistance = 0;
    [SerializeField] float drawAngle = 0;
    [SerializeField] float drawDuration = 0;

    float drawStartTime = 0;
    Vector3 finalDrawnLocalPosition;
    Quaternion finalDrawRotation;
    #endregion


    #region Swipe Phase Variables
    [Header("Swipe")]
    [SerializeField] float swipeDistance = 0;
    [SerializeField] float swipeAngle = 0;
    [SerializeField] float swipeDuration = 0;

    float swipeStartTime = 0;
    Vector3 finalSwipeLocalPosition;
    Quaternion finalSwipeRotation;
    #endregion


    #region Reset Phase Variables
    [Header("Reset")]
    [SerializeField] float resetDuration = 0;

    float resetStartTime = 0;
    #endregion


    void Start()
    {
        SetupSwipeTrail();
    }

    void Update()
    {
        if (featureState == EFeatureState.Disabled) { return; }

        //DEBUG
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

        Behavior();
    }

    void Behavior()
    {
        switch (attackPhase)
        {
            case EAttackPhase.Reading:
                ReadInput();
                break;

            case EAttackPhase.Draw:
                Draw();
                break;

            case EAttackPhase.Swipe:
                Swipe();
                break;

            case EAttackPhase.Reset:
                Reset();
                break;
        }
    }



    #region Reading Methods
    protected override void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Start attack input
            mouseSwipe = Vector3.zero;
            weaponController.skelyMovement.canRotate = false;

            //***ACTIVATING FEATURE***
            Activate();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Record mouse input
            mouseSwipe += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            weaponController.skelyMovement.canRotate = true;

            //If nonzero input, begin draw phase
            if (mouseSwipe.magnitude > 0.1)
            {
                BeginDrawPhase();
            }
        }
    }
    #endregion



    #region Draw Methods
    void BeginDrawPhase()
    {
        weaponController.DisableAnimator();
        mouseSwipe.Normalize();

        attackPhase = EAttackPhase.Draw;
        drawStartTime = Time.time;
    }

    void Draw()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration; //0-1

        DrawToPosition(drawProgress);
        DrawToRotation(drawProgress);

        //When done drawing
        if (drawProgress > 1)
        {
            BeginSwipePhase();
        }
    }

    float CalculateTargetBladeAngle()
    {
        //Only half of possible angles need to be accounted for because they are functionally the same
        //If sign of x is negative, flip signs of x and y
        float signAdjustment = (mouseSwipe.x < 0) ? -1 : 1;
        Vector3 adjustedMouseSwipe = signAdjustment * mouseSwipe;
        float targetZAngle = Mathf.Rad2Deg * Mathf.Atan(adjustedMouseSwipe.y / adjustedMouseSwipe.x);
        targetZAngle -= 90.0f;

        return targetZAngle;
    }

    void DrawToPosition(float drawProgress)
    {
        //Stage sword in opposite direction as mouseSwipe
        Vector3 drawDir = new Vector3(-mouseSwipe.x, -mouseSwipe.y, 0);

        //Calculate target world position
        Vector3 localDelta = new Vector3(drawDir.x * drawDistance, drawDir.y * drawDistance, 0);
        Vector3 targetLocalPos = weaponController.defaultPosition + localDelta;
        Transform skelyTransform = weaponController.ownerSkely.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);

        //Lerp position
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(weaponController.defaultPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);

        finalDrawnLocalPosition = targetLocalPos;
    }

    void DrawToRotation(float drawProgress)
    {
        Vector3 drawDir = new Vector3(-mouseSwipe.x, mouseSwipe.y, 0);

        //Calculate target local rotation
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        float targetZAngle = CalculateTargetBladeAngle();

        Quaternion deltaRotation =
            Quaternion.Euler(
            drawDir.y * drawAngle,
            drawDir.x * drawAngle,
            targetZAngle
            );

        Quaternion targetRotation = zeroRotation * deltaRotation;

        //Lerp sword rotation
        Quaternion initialRotation = Quaternion.Euler(weaponController.defaultRotation);
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, drawProgress);

        finalDrawRotation = targetRotation;
    }
    #endregion



    #region Swipe Methods
    void BeginSwipePhase()
    {
        attackPhase = EAttackPhase.Swipe;
        swipeStartTime = Time.time;
        swipeTrail.enabled = true;
    }

    void Swipe()
    {
        float swipeProgress = (Time.time - swipeStartTime) / swipeDuration; //0-1

        SwipeToPosition(swipeProgress);
        SwipeToRotation();

        //When done swiping
        if (swipeProgress > 1)
        {
            BeginResetPhase();
        }
    }

    void SwipeToPosition(float swipeProgress)
    {
        //Calculate target world position
        Vector3 localDelta = new Vector3(mouseSwipe.x * swipeDistance, mouseSwipe.y * swipeDistance, 0);
        Vector3 targetLocalPos = weaponController.defaultPosition + localDelta;
        Transform skelyTransform = weaponController.ownerSkely.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);

        //Lerp position
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(finalDrawnLocalPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, swipeProgress);

        finalSwipeLocalPosition = targetLocalPos;
    }

    void SwipeToRotation()
    {
        //Calculate rotation about local X-axis
        float rotateDirection = (mouseSwipe.x > 0) ? -1 : 1;
        float rotationRate = (Time.deltaTime * swipeAngle) / swipeDuration;
        float deltaX = rotateDirection * rotationRate;

        //Rotate deltaX per frame
        transform.Rotate(new Vector3(deltaX, 0, 0), Space.Self);
    }

    void SetupSwipeTrail()
    {
        if (!swipeTrail)
        {
            print("No swipe trail ref");
            return;
        }
        swipeTrail.enabled = false;
        swipeTrail.startWidth = 0.2f;
        swipeTrail.endWidth = 0.02f;
    }
    #endregion



    #region Reset Methods
    void BeginResetPhase()
    {
        swipeTrail.enabled = false;
        finalSwipeRotation = transform.localRotation;

        attackPhase = EAttackPhase.Reset;
        resetStartTime = Time.time;
    }

    void Reset()
    {
        float resetProgress = (Time.time - resetStartTime) / resetDuration;

        //Restore default position
        Transform skelyTransform = weaponController.ownerSkely.transform;
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(finalSwipeLocalPosition);
        transform.localPosition = Vector3.Lerp(finalSwipeLocalPosition, weaponController.defaultPosition, resetProgress);

        //Restore default rotation
        Quaternion initialRotation = Quaternion.Euler(weaponController.defaultRotation);
        transform.localRotation = Quaternion.Lerp(finalSwipeRotation, Quaternion.Euler(weaponController.defaultRotation), resetProgress);


        //Reset system
        if (resetProgress > 1)
        {
            attackPhase = EAttackPhase.Reading;
            weaponController.EnableAnimator();

            //*****Deactivate*****
            Deactivate();
        }
    }
    #endregion
}
