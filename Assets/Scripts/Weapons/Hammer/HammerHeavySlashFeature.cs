using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerHeavySlashFeature : SlashAttackFeature
{
    [Header("Knock Back")]
    [SerializeField] float knockBackForce = 3000;

    protected override void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            slashDirection = Vector3.down;
                
            //***ACTIVATING FEATURE***
            Activate();
        }
    }

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
        Vector3 knockBackDirection = playerTransform.forward + playerTransform.up;
            
        targetKnockBack.TakeKnockBack(knockBackDirection * knockBackForce);
    }
}
