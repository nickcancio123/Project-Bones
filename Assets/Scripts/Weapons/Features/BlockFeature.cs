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

    GameObject attacker = null;

    protected override void Update() => base.Update();

    protected override void SetInitialState() => initialState = null;

    public virtual float BlockAttack(float maxDamageAmount, int attackerID) => maxDamageAmount;

    protected void PlayBlockEffects(GameObject _attacker)
    {
        attacker = _attacker;
        photonView.RPC("RPC_PlayBlockEffects", RpcTarget.All);
    }

    [PunRPC]
    protected void RPC_PlayBlockEffects()
    {
        //Play Block SFX
        AudioSource weaponAudioSource = weaponController.weaponAudioSource;
        if (weaponAudioSource)
            weaponAudioSource.PlayOneShot(blockAudioClip);

        //Play particle system
        blockParticles.gameObject.SetActive(true);
        blockParticles.Play();
        
        //Knock back attacker
        KnockBack attackerKnockBack = attacker?.GetComponent<KnockBack>();
        if (attackerKnockBack)
        {
            Vector3 force = knockBackForce * weaponController.ownerPlayer.transform.forward;            
            attackerKnockBack.TakeKnockBack(force);
        }
    }
}
