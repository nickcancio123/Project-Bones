using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FeetAnimationManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject player;
    [SerializeField] Animator animator;
    [SerializeField] MovementManager movementManager;
    [SerializeField] GameObject pelvis;

    Vector3 forward;

    void Update()
    {
        UpdateAnimationParams();
        RotatePelvis();
    }

    void UpdateAnimationParams()
    {
        if (!animator)
        {
            print("No animator ref");
            return;
        }

        animator.SetFloat("velocity", movementManager.GetSpeed());
        animator.SetBool("isRunning", movementManager.isRunning);
    }

    void RotatePelvis()
    {
        Vector3 velocity = movementManager.characterController.velocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        if (photonView.IsMine)
        {
            if (horizontalVelocity.magnitude > 0.1f)
                forward = horizontalVelocity.normalized;
            else
                forward = Vector3.Lerp(forward, player.transform.forward, 0.5f);
        }

        pelvis.transform.forward = forward;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(forward.x);
            stream.SendNext(forward.y);
            stream.SendNext(forward.z);
        }
        else
        {
            forward.x = (float)stream.ReceiveNext();
            forward.y = (float)stream.ReceiveNext();
            forward.z = (float)stream.ReceiveNext();
        }
    }
}
