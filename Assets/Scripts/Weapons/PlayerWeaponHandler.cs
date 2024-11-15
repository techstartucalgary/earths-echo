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
    private MeleeWeapon MeleeWeapon;
    private ProjectileWeapon ProjectileWeapon;


    void Update()
    {
        // Check for key presses to switch weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Destroy(activeWeaponInstance);
            ActivateWeapon(WeaponType.Melee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Destroy(activeWeaponInstance);
            ActivateWeapon(WeaponType.Projectile);
        }

        // Call attack handling method
        AttackByType();
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
        if (activeWeaponType == weaponType) return;

        if (activeWeaponInstance != null)
        {
            Destroy(activeWeaponInstance);
            activeWeaponInstance = null;
            MeleeWeapon = null;
            ProjectileWeapon = null;
        }

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

        if (weaponPrefab != null && slot != null)
        {
            activeWeaponInstance = Instantiate(weaponPrefab, slot.position, Quaternion.identity, slot);
            activeWeaponInstance.GetComponent<Weapon>()?.Equip(transform);

            if (weaponType == WeaponType.Melee)
                MeleeWeapon = activeWeaponInstance.GetComponent<MeleeWeapon>();
            else if (weaponType == WeaponType.Projectile)
                ProjectileWeapon = activeWeaponInstance.GetComponent<ProjectileWeapon>();

            Debug.Log("Activated " + weaponType.ToString().ToLower() + " weapon: " + activeWeaponInstance.name);
            activeWeaponType = weaponType;
        }
        else
        {
            Debug.LogWarning("No weapon prefab set for " + weaponType.ToString().ToLower() + " weapon.");
            activeWeaponType = WeaponType.None;
        }
    }

    private void AttackByType()
    {
        switch (activeWeaponType)
        {
            case WeaponType.Melee:
                if (Input.GetMouseButtonDown(0) && MeleeWeapon != null)
                {
                    MeleeWeapon.PrimaryAttack();
                }
                if (Input.GetMouseButtonDown(1) && MeleeWeapon != null && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                {
                    MeleeWeapon.SideAttack();
                }
                if (Input.GetKey(KeyCode.S) && Input.GetMouseButtonDown(1) && MeleeWeapon != null)
                {
                    MeleeWeapon.DownAttack();
                }
                if (Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(1) && MeleeWeapon != null)
                {
                    MeleeWeapon.UpAttack();
                }
                break;

            case WeaponType.Projectile:
                if (Input.GetMouseButtonDown(0) && ProjectileWeapon != null)
                {
                    ProjectileWeapon.PrimaryAttack();
                }
                break;

            case WeaponType.None:
                break;
        }
    }
}
