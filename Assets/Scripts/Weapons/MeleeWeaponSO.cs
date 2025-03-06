using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeWeapon", menuName = "Weapon/Melee Weapon")]
public class MeleeWeaponSO : WeaponSO
{
    [Header("Melee Weapon Stats")]
    [Tooltip("The cooldown time between swings for this melee weapon.")]
    public float swingSpeed = 0.5f;
    
}
