using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CullSelfFromCamera : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!photonView.IsMine) { return; }

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Self"));
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}
