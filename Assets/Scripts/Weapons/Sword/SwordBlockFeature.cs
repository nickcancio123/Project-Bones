using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordBlockFeature : BlockFeature
{
    public Vector3 defaultBlockPosition;
    public Vector3 defaultBlockRotation;
    public Transform swordMidPoint;
    [SerializeField] float maxIncidentAngleToHit = 1;
    [SerializeField] float maxAngleToFaceAttackerToBeHit = 1;
    [HideInInspector] public float blockAngle = 0;

    [Header("Draw")]
    public float drawDuration = 1;

    [Header("Block")]
    public float blockRotationSpeed = 1;
    public float blockLockOnSpeed = 1;


    protected override void SetInitialState() => initialState = gameObject.AddComponent<Sword_Block_Draw_State>();

    protected override void Update()
    {
        if (featurePhase == EFeaturePhase.Disabled) { return; }
        if (!photonView.IsMine) { return; }

        ReadInput();
    }

    void ReadInput()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //***ACTIVATING FEATURE***
            Activate();
        }
    }

    #region Block Functionality
    public override float BlockAttack(float maxDamageAmount, int attackerID)
    {
        if (!isBlocking) { return maxDamageAmount; }

        float damageTaken = maxDamageAmount;

        PhotonView attackerPV = PhotonView.Find(attackerID);
        attacker = attackerPV.gameObject;
        GameObject weapon = attacker.GetComponentInChildren<IWeapon>()?.gameObject;
        attackFeature = weapon?.GetComponentInChildren<AttackFeature>();
        Vector3 attackerPosition = attackerPV.gameObject.transform.position;
        
        switch (attackFeature.attackType)
        {
            case AttackType.Slash:
                {
                    damageTaken = BlockSlashAttack(maxDamageAmount, attackerPosition);
                    break;
                }

            case AttackType.Jab:
                {
                    //No jab attacks yet
                    break;
                }
        }

        //If block successful
        if (damageTaken == 0)   
            PlayBlockEffects(attackerID);

        return damageTaken;
    }

    float BlockSlashAttack(float maxDamageAmount, Vector3 attackerPosition)
    {
        //If not facing enemy, take full damage
        Transform playerTransform = weaponController.ownerPlayer.transform;

        Vector3 directionToAttacker = attackerPosition - playerTransform.position;
        float deltaAngleToFaceAttacker = Vector3.Angle(playerTransform.forward, directionToAttacker);

        if (deltaAngleToFaceAttacker > maxAngleToFaceAttackerToBeHit)
        {
            //Your back is to the attacker, you did not block the attack
            return maxDamageAmount;
        }

        //Negated to make local space
        float attackAngle = attackFeature.attackAngle;
        
        float incidentAngle = Mathf.Abs(blockAngle - attackAngle);

        if (incidentAngle > 90)
            incidentAngle = Mathf.Abs(180 - incidentAngle);

        bool gotHit = incidentAngle < maxIncidentAngleToHit;
        
        return (gotHit) ? maxDamageAmount : 0;
    }
    #endregion
}
