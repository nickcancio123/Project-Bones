using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponInfo : MonoBehaviour
{
    [HideInInspector] public static EWeaponClass selectedWeaponClass = EWeaponClass.Sword;
    [HideInInspector] public static GameObject selectedWeaponPrefab = null;
}
