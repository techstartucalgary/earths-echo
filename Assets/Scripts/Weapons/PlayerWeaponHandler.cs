using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private Weapon equippedMeleeWeapon;
    private Weapon equippedProjectileWeapon;

    public Transform equippedMeleeSlot;
    public Transform equippedProjectileSlot;

    public void EquipMeleeWeapon(GameObject weaponPrefab)
    {
        if (equippedMeleeWeapon != null)
        {
            UnequipWeapon(equippedMeleeWeapon);
        }

        GameObject weaponInstance = Instantiate(weaponPrefab, equippedMeleeSlot.position, Quaternion.identity, equippedMeleeSlot);
        equippedMeleeWeapon = weaponInstance.GetComponent<MeleeWeapon>();

        if (equippedMeleeWeapon != null)
        {
            equippedMeleeWeapon.Equip(transform);
            Debug.Log("Equipped melee weapon: " + equippedMeleeWeapon.weaponName);
        }
    }

    public void EquipProjectileWeapon(GameObject weaponPrefab)
    {
        if (equippedProjectileWeapon != null)
        {
            UnequipWeapon(equippedProjectileWeapon);
        }

        GameObject weaponInstance = Instantiate(weaponPrefab, equippedProjectileSlot.position, Quaternion.identity, equippedProjectileSlot);
        equippedProjectileWeapon = weaponInstance.GetComponent<ProjectileWeapon>();

        if (equippedProjectileWeapon != null)
        {
            equippedProjectileWeapon.Equip(transform);
            Debug.Log("Equipped projectile weapon: " + equippedProjectileWeapon.weaponName);
        }
    }

    private void UnequipWeapon(Weapon weapon)
    {
        weapon.Unequip();
        Destroy(weapon.gameObject);
    }
}
