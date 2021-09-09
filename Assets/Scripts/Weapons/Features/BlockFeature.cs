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

    protected bool isBlocking = false;
    public bool IsBlocking() { return isBlocking; }


    protected override void SetInitialState()
    {
        initialState = null;
    }

    public virtual float BlockAttack(float maxDamageAmount, int attackerID)
    {
        //Returns damage to be taken
        return maxDamageAmount;
    }

    protected void PlayBlockEffects()
    {
        photonView.RPC("RPC_PlayBlockEffects", RpcTarget.All, "Block");
    }

    [PunRPC]
    protected void RPC_PlayBlockEffects(string blockAudioClipName)
    {
        //Play Block SFX
        WeaponController weaponController = gameObject.GetComponent<WeaponController>();
        AudioSource weaponAudioSource = weaponController?.weaponAudioSource;
        weaponAudioSource.PlayOneShot(blockAudioClip);

        //Play particle system
        blockParticles.gameObject.SetActive(true);
        blockParticles.Play();
    }
}
