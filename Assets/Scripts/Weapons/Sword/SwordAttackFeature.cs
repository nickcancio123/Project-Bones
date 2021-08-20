using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
>Blade swipe direction should be in local Y-plane
*/

public class SwordAttackFeature : WeaponFeature
{
    #region General
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

    [SerializeField] float drawDistance = 0;
    [SerializeField] float drawAngle = 0;
    [SerializeField] float drawDuration = 0;

    float drawStartTime = 0;
    Transform swordBeforeStage;

    #endregion



    void Start()
    {

    }

    void Update()
    {
        if (featureState == EFeatureState.Disabled) { return; }

        //DEBUG
        if (Input.GetKeyDown(KeyCode.R))
        {
            print("Reset");
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


    protected override void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Start attack input
            mouseSwipe = Vector3.zero;
            weaponController.skelyMovement.canRotate = false;

            //*****
            Activate();
            //*****
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

    #region Draw Methods
    void BeginDrawPhase()
    {
        weaponController.DisableAnimator();

        mouseSwipe.Normalize();
        attackPhase = EAttackPhase.Draw;
        drawStartTime = Time.time;
        swordBeforeStage = transform;
    }

    void Draw()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration; //0-1

        DrawToPosition(drawProgress);
        DrawToRotation(drawProgress);

        //Complete draw
        if (drawProgress > 1)
        {
            attackPhase = EAttackPhase.Swipe;
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
        Vector3 targetLocalPos = weaponController.defaultPosition + new Vector3(drawDir.x * drawDistance, drawDir.y * drawDistance, 0);
        Transform skelyTransform = weaponController.ownerSkely.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);

        //Lerp position
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(weaponController.defaultPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);
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
        Quaternion initialRotation = swordBeforeStage.localRotation;
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, drawProgress);
    }

    #endregion

    void Swipe()
    {

    }

    void Reset()
    {
        transform.localRotation = Quaternion.Euler(weaponController.defaultRotation);
        attackPhase = EAttackPhase.Reading;

        weaponController.EnableAnimator();
    }
}
