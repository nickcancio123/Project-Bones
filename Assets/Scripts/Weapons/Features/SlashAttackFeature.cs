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
    protected Vector3 mouseSwipe = Vector3.zero;
    public bool canDealDamage = false;

    [Header("Draw")]
    public float drawDistance = 0;
    public float drawAngle = 0;
    public float drawDuration = 0;

    [Header("Slash")]
    public float slashDistance = 0;
    public float slashAngle = 0;
    public float slashDuration = 0;

    [Header("Reset")]
    public float resetDuration = 0;


    void Start()
    {
        attackType = AttackType.Slash;
        slashTrails.SetActive(false);
    }

    protected override void SetInitialState()
    {
        initialState = gameObject.AddComponent<Slash_Draw_State>();
    }

    void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Start attack input
            mouseSwipe = Vector3.zero;
            weaponController.skelyMovement.canRotate = false;
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
                //***ACTIVATING FEATURE***
                Activate();
            }
        }
    }

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
        if (other.gameObject == weaponController.ownerSkely) { return; }

        if (other.gameObject.tag == "Player")
        {
            Attack(other.gameObject);
        }
    }
    #endregion

}