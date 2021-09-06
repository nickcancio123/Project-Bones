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

        if (photonView.IsMine)
            UpdateParticles();

        Synchronize();
    }

    void UpdateParticles()
    {
        float speed = movementManager.characterController.velocity.magnitude;

        if (movementManager.isRunning && movementManager.characterController.isGrounded && speed > 1)
            emitting = true;
        else
            emitting = false;
    }

    void Synchronize()
    {
        if (emitting)
            particles.Play();
        else
            particles.Stop();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(emitting);
        }
        else
        {
            emitting = (bool)stream.ReceiveNext();
        }
    }

}
