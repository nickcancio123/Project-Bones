using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponClass
{
    Sword,
    Hammer
}

public class WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    
    [SerializeField] List<GameObject> weaponPrefabs;
    Dictionary<EWeaponClass, GameObject> weaponClassToPrefab = new Dictionary<EWeaponClass, GameObject>();
    
    private GameObject selectedWeaponPrefab;

    void Start() => InitializePrefabDictionary();

    void InitializePrefabDictionary()
    {
        for (int i = 0; i < weaponPrefabs.Count; i++)
            weaponClassToPrefab.Add(IntToWeaponClass(i), weaponPrefabs[i]);
        
        SelectWeaponClass("Sword"); //TEMP*****
    }
    
    public void SelectWeaponClass(string weaponName)
    {
        EWeaponClass selectedWeaponClass = StringToWeaponClass(weaponName);
        PlayerWeaponInfo.selectedWeaponClass = selectedWeaponClass;
        weaponClassToPrefab.TryGetValue(selectedWeaponClass, out PlayerWeaponInfo.selectedWeaponPrefab);
    }

    static EWeaponClass StringToWeaponClass(string weaponName)
    {
        switch (weaponName)
        {
            case "Sword":
                return EWeaponClass.Sword;
            case "Hammer":
                return EWeaponClass.Hammer;
            default:
                return EWeaponClass.Sword;
        }
    }
    
    static EWeaponClass IntToWeaponClass(int i)
    {
        switch (i)
        {
            case 0:
                return EWeaponClass.Sword;
            case 1:
                return EWeaponClass.Hammer;
            default:
                return EWeaponClass.Sword;
        }
    }
}
