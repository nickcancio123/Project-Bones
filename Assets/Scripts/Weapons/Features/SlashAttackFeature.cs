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
    public GameObject slashTrails;
    [HideInInspector] public Vector3 mouseSwipe = Vector3.zero;

    [Header("Draw")]
    public float drawDistance = 0;
    public float drawAngle = 0;
    public float drawDuration = 0;

    [Header("Slash")]
    public float slashDistance = 0;
    public float slashAngle = 0;
    public float slashDuration = 0;

    Slash_Draw_State draw_State;

    void Start()
    {
        attackType = AttackType.Slash;
        slashTrails.SetActive(false);
        weaponController.collisionEvent += OnWeaponCollision;
    }

    protected override void SetInitialState()
    {
        draw_State = gameObject.AddComponent<Slash_Draw_State>();
        initialState = draw_State;
    }

    protected override void Update()
    {
        base.Update();

        if (!photonView.IsMine) { return; }
        if (featurePhase == EFeaturePhase.Disabled) { return; }

        ReadInput();
    }

    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseSwipe = Vector3.zero;
            weaponController.movementManager.canRotate = false;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            mouseSwipe += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            weaponController.movementManager.canRotate = true;

            if (mouseSwipe.magnitude > 0.1)
            {
                mouseSwipe.Normalize();
                //***ACTIVATING FEATURE***
                Activate();
            }
        }
    }


    public new void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        if (stream.IsWriting)
        {
            stream.SendNext(slashTrails.activeInHierarchy);
        }
        else
        {
            if (!photonView.IsMine)
            {
                slashTrails.SetActive((bool)stream.ReceiveNext());
            }
        }
    }

    public override void OnWeaponCollision(Collider other)
    {
        if (!photonView.IsMine) { return; }
        if (!canDealDamage) { return; }
        if (other.gameObject == weaponController.ownerPlayer) { return; }
        if (other.gameObject.CompareTag("Player"))
        {
            Attack(other.gameObject);
        }
    }
}