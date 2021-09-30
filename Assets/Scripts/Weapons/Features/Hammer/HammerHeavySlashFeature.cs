using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HammerHeavySlashFeature : SlashAttackFeature, IPunObservable
{
    public ParticleSystem groundImpactParticles;
    [HideInInspector] public bool canImpactGround = false;

    [Header("Knock Back")]
    [SerializeField] float knockBackForce = 3000;

    protected override void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            slashDirection = Vector3.down;
            //***ACTIVATING FEATURE***
            Activate();
        }
    }

    protected override void SetInitialState() => initialState = gameObject.AddComponent<Hammer_HeavySlash_Draw_State>();

    protected override void Attack(GameObject targetPlayer)
    {
        base.Attack(targetPlayer);
        ApplyKnockBack(targetPlayer);
    }
    
    void ApplyKnockBack(GameObject targetPlayer)
    {
        KnockBack targetKnockBack = targetPlayer.GetComponent<KnockBack>();
        if (!targetKnockBack) { return; }

        Transform playerTransform = weaponController.ownerPlayer.transform;
        Vector3 knockBackDirection = playerTransform.forward + playerTransform.up;
            
        targetKnockBack.TakeKnockBack(knockBackDirection * knockBackForce, unblockable);
    }

    

    public override void OnWeaponCollision(Collider other)
    {
        base.OnWeaponCollision(other);
        
        if (!canImpactGround)
            return;
        if (!photonView.IsMine)
            return;
        
        if (other.gameObject.CompareTag("Ground"))
        {
            photonView.RPC("RPC_PlayGroundImpactEffects", RpcTarget.All);
        }
    }
    
    [PunRPC]
    void RPC_PlayGroundImpactEffects() => groundImpactParticles.Play();
}
