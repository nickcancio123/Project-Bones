using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
>Attack feature broken into 3 phases (not including reading state)
    -Draw: the backswing of the attack
    -Slash: the actual swing of the sword
    -Reset: moving the sword to its default state
>Slash direction should align with local Y-axis. Then rotate around X-axis to swing
*/

public class SlashAttackFeature : AttackFeature
{
    #region General
    [SerializeField] GameObject slashTrails;

    protected enum EAttackPhase
    {
        Reading,
        Draw,
        Slash,
        Reset
    }

    protected EAttackPhase attackPhase = EAttackPhase.Reading;

    protected Vector3 mouseSwipe = Vector3.zero;

    protected bool canDealDamage = false;
    #endregion



    #region Draw Phase Variables
    [Header("Draw")]
    [SerializeField] protected float drawDistance = 0;
    [SerializeField] protected float drawAngle = 0;
    [SerializeField] protected float drawDuration = 0;

    protected float drawStartTime = 0;
    protected Vector3 drawnLocalPosition;
    protected Quaternion drawnLocalRotation;
    #endregion



    #region Slash Phase Variables
    [Header("Slash")]
    [SerializeField] protected float slashDistance = 0;
    [SerializeField] protected float slashAngle = 0;
    [SerializeField] protected float slashDuration = 0;

    protected float slashStartTime = 0;
    protected Vector3 finalSlashLocalPosition;
    protected Quaternion finalSlashLocalRotation;
    #endregion



    #region Reset Phase Variables
    [Header("Reset")]
    [SerializeField] protected float resetDuration = 0;

    protected float resetStartTime = 0;
    #endregion



    #region General Methods
    void Start()
    {
        attackType = AttackType.Slash;
        SetupSlashTrail();
    }

    void Update()
    {
        if (featureState == EFeatureState.Disabled) { return; }
        if (!photonView.IsMine) { return; }

        Behavior();
    }

    void Behavior()
    {
        if (stopDefaultBehavior)
        {
            if (isRecoiling)
                RecoilFromBlock();
            return;
        }

        //Default behavior
        switch (attackPhase)
        {
            case EAttackPhase.Reading:
                ReadInput();
                break;

            case EAttackPhase.Draw:
                Draw();
                break;

            case EAttackPhase.Slash:
                Slash();
                break;

            case EAttackPhase.Reset:
                Reset();
                break;
        }
    }
    #endregion



    #region Reading Methods
    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Start attack input
            mouseSwipe = Vector3.zero;
            weaponController.movementManager.canRotate = false;

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
            weaponController.movementManager.canRotate = true;

            //If nonzero input, begin draw phase
            if (mouseSwipe.magnitude > 0.1)
            {
                BeginDrawPhase();
            }
            else
            {
                BeginResetPhase();
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

        CalculateAttackAngle();
    }

    void Draw()
    {
        float drawProgress = (Time.time - drawStartTime) / drawDuration; //0-1

        DrawToPosition(drawProgress);
        DrawToRotation(drawProgress);

        //When done drawing
        if (drawProgress > 1)
        {
            BeginSlashPhase();
        }
    }

    float CalculateTargetSlashAngle()
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
        //Draw sword in opposite direction as mouseSwipe
        Vector3 drawDir = new Vector3(-mouseSwipe.x, -mouseSwipe.y, 0);

        //Calculate target world position
        Vector3 localDelta = new Vector3(drawDir.x * drawDistance, drawDir.y * drawDistance, 0);
        Vector3 targetLocalPos = weaponController.defaultPosition + localDelta;
        Transform skelyTransform = weaponController.ownerPlayer.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);

        //Lerp position
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(weaponController.defaultPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, drawProgress);

        drawnLocalPosition = targetLocalPos;
    }

    void DrawToRotation(float drawProgress)
    {
        Vector3 drawDir = new Vector3(-mouseSwipe.x, mouseSwipe.y, 0);

        //Calculate target local rotation
        Quaternion zeroRotation = Quaternion.Euler(0, 0, 0);
        float targetZAngle = CalculateTargetSlashAngle();

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

        drawnLocalRotation = targetRotation;
    }

    void CalculateAttackAngle()
    {
        attackAngle = Mathf.Rad2Deg * Mathf.Atan2(mouseSwipe.x, mouseSwipe.y);
    }
    #endregion



    #region Slash Methods
    void BeginSlashPhase()
    {
        attackPhase = EAttackPhase.Slash;
        canDealDamage = false;

        slashStartTime = Time.time;
        slashTrails.SetActive(true);
    }

    void Slash()
    {
        float slashProgress = (Time.time - slashStartTime) / slashDuration; //0-1

        SlashToPosition(slashProgress);
        SlashToRotation();

        //When done slash
        if (slashProgress > 1)
        {
            BeginResetPhase();
        }
    }

    void SlashToPosition(float slashProgress)
    {
        //Calculate target world position
        Vector3 localDelta = new Vector3(mouseSwipe.x * slashDistance, mouseSwipe.y * slashDistance, 0);
        Vector3 targetLocalPos = weaponController.defaultPosition + localDelta;
        Transform skelyTransform = weaponController.ownerPlayer.transform;
        Vector3 targetWorldPos = skelyTransform.position + skelyTransform.TransformDirection(targetLocalPos);

        //Lerp position
        Vector3 startingPos = skelyTransform.position + skelyTransform.TransformDirection(drawnLocalPosition);
        transform.position = Vector3.Lerp(startingPos, targetWorldPos, slashProgress);
    }

    void SlashToRotation()
    {
        //Calculate rotation about local X-axis
        float rotateDirection = (mouseSwipe.x > 0) ? -1 : 1;
        float rotationRate = (Time.deltaTime * slashAngle) / slashDuration;
        float deltaX = rotateDirection * rotationRate;

        //Rotate deltaX per frame
        transform.Rotate(new Vector3(deltaX, 0, 0), Space.Self);
    }

    void SetupSlashTrail()
    {
        if (!slashTrails)
        {
            print("No slash trail ref");
            return;
        }
        slashTrails.SetActive(false);
    }
    #endregion



    #region Reset Methods
    void BeginResetPhase()
    {
        slashTrails.SetActive(false);
        finalSlashLocalRotation = transform.localRotation;
        finalSlashLocalPosition = transform.localPosition;

        attackPhase = EAttackPhase.Reset;
        resetStartTime = Time.time;
    }

    void Reset()
    {
        float resetProgress = (Time.time - resetStartTime) / resetDuration;

        //Restore default position
        transform.localPosition = Vector3.Lerp(finalSlashLocalPosition, weaponController.defaultPosition, resetProgress);

        //Restore default rotation
        transform.localRotation =
            Quaternion.Lerp(finalSlashLocalRotation, Quaternion.Euler(weaponController.defaultRotation), resetProgress);

        //Reset system 
        if (resetProgress > 1)
        {
            attackPhase = EAttackPhase.Reading;
            weaponController.EnableAnimator();

            //*****DEACTIVATE FEATURE*****
            Deactivate();
        }
    }
    #endregion



    #region Recoil Methods
    override protected void RecoilFromBlock()
    {
        float percentRecoiled = (Time.time - recoilStartTime / recoilDuration);

        transform.localPosition = Vector3.Lerp(recoilStartLocalPos, drawnLocalPosition, percentRecoiled);
        transform.localRotation = Quaternion.Lerp(recoilStartLocalRotation, drawnLocalRotation, percentRecoiled);

        if (percentRecoiled >= 1)
        {
            stopDefaultBehavior = false;

            BeginResetPhase();
        }
    }
    #endregion



    #region Attack Network Interaction
    new public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        if (stream.IsWriting)
        {
            stream.SendNext(slashTrails.activeInHierarchy);
        }
        else
        {
            slashTrails.SetActive((bool)stream.ReceiveNext());
        }
    }

    public override void OnWeaponCollision(Collider other)
    {
        if (!photonView.IsMine) { return; }

        if (!canDealDamage) { return; }

        //Ignore if weapon collides with self
        if (other.gameObject == weaponController.ownerPlayer) { return; }

        //If weapon collided with another player or child of another player
        if (other.gameObject.tag == "Player")
        {
            Attack(other.gameObject);
        }
    }
    #endregion
}
