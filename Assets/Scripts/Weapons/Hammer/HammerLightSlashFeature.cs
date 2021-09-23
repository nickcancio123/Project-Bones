using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerLightSlashFeature : SlashAttackFeature
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
        Vector3 right = playerTransform.right * slashDirection.x;
        Vector3 up = playerTransform.up * (1 + ((slashDirection.y > 0) ? slashDirection.y : 0));    //Swipe up pushes player more up
        Vector3 forward = playerTransform.forward * ((slashDirection.y < 0) ? -slashDirection.y : 0);  //Swipe down pushes player back

        Vector3 knockBackDirection = forward + right + up;
            
        targetKnockBack.TakeKnockBack(knockBackDirection * knockBackForce);
    }
}
