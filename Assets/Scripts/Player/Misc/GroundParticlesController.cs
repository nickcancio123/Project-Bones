using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GroundParticlesController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] MovementManager movementManager;

    ParticleSystem particles;
    bool emitting = false;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!particles) { return; }

        UpdateParticles();
    }

    bool ShouldEmit()
    {
        float speed = movementManager.characterController.velocity.magnitude;

        bool shouldEmit = (movementManager.isRunning && movementManager.characterController.isGrounded && speed > 1);
        return shouldEmit;
    }

    void UpdateParticles()
    {
        if (photonView.IsMine)
            emitting = ShouldEmit();

        if (emitting)
        {
            if (!particles.isPlaying)
                particles.Play();
        }
        else
        {
            if (!particles.isStopped)
                particles.Stop();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ShouldEmit());
        }
        else
        {
            emitting = (bool)stream.ReceiveNext();
        }
    }
}
