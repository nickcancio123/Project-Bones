using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerSlashFeature : SlashAttackFeature
{
    [Header("Knock Back")]
    [SerializeField] float knockBackForce = 1000;
    
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
        Vector3 right = playerTransform.right * mouseSwipe.x;
        Vector3 up = playerTransform.up * ((mouseSwipe.y > 0) ? mouseSwipe.y : 0);    //Swipe up pushes player up
        Vector3 forward = playerTransform.forward * ((mouseSwipe.y < 0) ? -mouseSwipe.y : 0);  //Swipe down pushes player back

        Vector3 force = forward + right + up;
            
        targetKnockBack.TakeKnockBack(force * knockBackForce);
    }
}
