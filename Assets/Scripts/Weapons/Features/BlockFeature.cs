using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class BlockFeature : WeaponFeature
{
    [SerializeField] List<string> audioClipNames;
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] AudioClip blockAudioClip;
    [SerializeField] ParticleSystem blockParticles;
    [SerializeField] float knockBackForce = 600;
    
    
    [HideInInspector] public bool isBlocking = false;

    protected GameObject attacker = null;
    protected AttackFeature attackFeature = null;

    protected override void Update() => base.Update();

    protected override void SetInitialState() => initialState = null;

    public virtual float BlockAttack(float maxDamageAmount, int attackerID) => maxDamageAmount;

    
    
    protected void PlayBlockEffects(int attackerID) => photonView.RPC("RPC_PlayBlockEffects", RpcTarget.All, attackerID);

    [PunRPC]
    protected void RPC_PlayBlockEffects(int attackerID)
    {
        if (photonView.IsMine)
        {
            //Initiate attacker recoil
            PhotonView.Find(attackerID).RPC("RPC_BeginRecoil", RpcTarget.All, knockBackForce);
        }
        
        //Play Block SFX
        AudioSource weaponAudioSource = weaponController.weaponAudioSource;
        if (weaponAudioSource)
            weaponAudioSource.PlayOneShot(blockAudioClip);

        //Play particle effects
        blockParticles.gameObject.SetActive(true);
        blockParticles.Play();
    }
}
