using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private GameObject meleeWeaponPrefab; // Prefab for the melee weapon
    private GameObject projectileWeaponPrefab; // Prefab for the projectile weapon
    private GameObject activeWeaponInstance; // Currently active weapon instance

    public Transform meleeSlot; // Slot transform for melee weapon
    public Transform projectileSlot; // Slot transform for projectile weapon

    private enum WeaponType { None, Melee, Projectile }
    private WeaponType activeWeaponType = WeaponType.None;

    void Update()
    {
        // Check for key presses to switch weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateWeapon(WeaponType.Melee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateWeapon(WeaponType.Projectile);
        }
    }

    public void EquipMeleeWeapon(GameObject weaponPrefab)
    {
        meleeWeaponPrefab = weaponPrefab;
        // Automatically activate melee if it’s already the selected slot
        if (activeWeaponType == WeaponType.Melee)
        {
            ActivateWeapon(WeaponType.Melee);
        }
    }

    public void EquipProjectileWeapon(GameObject weaponPrefab)
    {
        projectileWeaponPrefab = weaponPrefab;
        // Automatically activate projectile if it’s already the selected slot
        if (activeWeaponType == WeaponType.Projectile)
        {
            ActivateWeapon(WeaponType.Projectile);
        }
    }

    private void ActivateWeapon(WeaponType weaponType)
    {
        // If the selected weapon is already active, do nothing
        if (activeWeaponType == weaponType) return;

        // Destroy the currently active weapon instance if it exists
        if (activeWeaponInstance != null)
        {
            Destroy(activeWeaponInstance);
            activeWeaponInstance = null;
        }

        // Instantiate the new weapon based on the selected type
        GameObject weaponPrefab = null;
        Transform slot = null;

        if (weaponType == WeaponType.Melee && meleeWeaponPrefab != null)
        {
            weaponPrefab = meleeWeaponPrefab;
            slot = meleeSlot;
        }
        else if (weaponType == WeaponType.Projectile && projectileWeaponPrefab != null)
        {
            weaponPrefab = projectileWeaponPrefab;
            slot = projectileSlot;
        }

        // Instantiate the weapon if a valid prefab is provided
        if (weaponPrefab != null && slot != null)
        {
            activeWeaponInstance = Instantiate(weaponPrefab, slot.position, Quaternion.identity, slot);
            activeWeaponInstance.GetComponent<Weapon>()?.Equip(transform);
            Debug.Log("Activated " + weaponType.ToString().ToLower() + " weapon: " + activeWeaponInstance.name);
            activeWeaponType = weaponType;
        }
        else
        {
            Debug.LogWarning("No weapon prefab set for " + weaponType.ToString().ToLower() + " weapon.");
            activeWeaponType = WeaponType.None; // Reset to None if instantiation fails
        }
    }
}
