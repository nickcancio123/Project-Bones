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

    public bool isBlocking = false;

    protected override void Update() => base.Update();

    protected override void SetInitialState() => initialState = null;

    public virtual float BlockAttack(float maxDamageAmount, int attackerID) => maxDamageAmount;

    protected void PlayBlockEffects()
    {
        photonView.RPC("RPC_PlayBlockEffects", RpcTarget.All, "Block");
    }

    [PunRPC]
    protected void RPC_PlayBlockEffects(string blockAudioClipName)
    {
        //Play Block SFX
        AudioSource weaponAudioSource = weaponController.weaponAudioSource;
        if (weaponAudioSource)
            weaponAudioSource.PlayOneShot(blockAudioClip);

        //Play particle system
        blockParticles.gameObject.SetActive(true);
        blockParticles.Play();
    }
}
