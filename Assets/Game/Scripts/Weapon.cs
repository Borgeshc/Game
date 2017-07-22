using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Unarmed,
        Sword,
        Staff,
        Spell,
        Spear,
        Shield,
        Rifle,
        Pistol,
        Mace,
        Dagger,
        TwoHandedSword,
        TwoHandedSpear,
        TwoHandedCrossbow,
        TwoHandedClub,
        TwoHandedBow,
        TwoHandedAxe
    };

    public WeaponType weaponType = WeaponType.Unarmed;
    public string weaponTypeName;
}
