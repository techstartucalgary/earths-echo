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
        HandleWeaponSwitching();
        AttackByType();
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // If melee weapon is already active, do nothing
            if (activeWeaponType == WeaponType.Melee)
            {
                Debug.Log("Melee weapon is already active.");
                return;
            }

            ActivateWeapon(WeaponType.Melee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // If projectile weapon is already active, do nothing
            if (activeWeaponType == WeaponType.Projectile)
            {
                Debug.Log("Projectile weapon is already active.");
                return;
            }

            ActivateWeapon(WeaponType.Projectile);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Put away any active weapon
            UnequipWeapon();
        }
    }

    public void EquipMeleeWeapon(GameObject weaponPrefab)
    {
        if (meleeWeaponPrefab == weaponPrefab)
        {
            Debug.Log("The same melee weapon is already equipped.");
            return;
        }

        meleeWeaponPrefab = weaponPrefab;

        // If melee is currently active, replace the weapon
        if (activeWeaponType == WeaponType.Melee)
        {
            ActivateWeapon(WeaponType.Melee);
        }
    }

    public void EquipProjectileWeapon(GameObject weaponPrefab)
    {
        if (projectileWeaponPrefab == weaponPrefab)
        {
            Debug.Log("The same projectile weapon is already equipped.");
            return;
        }

        projectileWeaponPrefab = weaponPrefab;

        // If projectile is currently active, replace the weapon
        if (activeWeaponType == WeaponType.Projectile)
        {
            ActivateWeapon(WeaponType.Projectile);
        }
    }

    private void ActivateWeapon(WeaponType weaponType)
    {
        // Unequip the current weapon if switching types or replacing the same type
        if (activeWeaponType != WeaponType.None)
        {
            UnequipWeapon();
        }

        GameObject weaponPrefab = null;

        if (weaponType == WeaponType.Melee && meleeWeaponPrefab != null)
        {
            weaponPrefab = meleeWeaponPrefab;
        }
        else if (weaponType == WeaponType.Projectile && projectileWeaponPrefab != null)
        {
            weaponPrefab = projectileWeaponPrefab;
        }

        if (weaponPrefab != null)
        {
            Transform weaponSlot = weaponType == WeaponType.Melee ? meleeSlot : projectileSlot;

            // Instantiate the new weapon in the correct slot
            activeWeaponInstance = Instantiate(weaponPrefab, weaponSlot.position, weaponSlot.rotation, weaponSlot);
            activeWeaponInstance.GetComponent<Weapon>()?.Equip(transform);

            if (weaponType == WeaponType.Melee)
            {
                MeleeWeapon = activeWeaponInstance.GetComponent<MeleeWeapon>();
            }
            else if (weaponType == WeaponType.Projectile)
            {
                ProjectileWeapon = activeWeaponInstance.GetComponent<ProjectileWeapon>();
            }

            activeWeaponType = weaponType;
            Debug.Log($"Activated {weaponType} weapon: {activeWeaponInstance.name}");
        }
        else
        {
            Debug.LogWarning($"No weapon prefab set for {weaponType} weapon.");
            activeWeaponType = WeaponType.None;
        }
    }

    private void UnequipWeapon()
    {
        if (activeWeaponInstance != null)
        {
            Debug.Log($"Unequipping weapon: {activeWeaponInstance.name}");
            Destroy(activeWeaponInstance);
        }

        activeWeaponInstance = null;
        MeleeWeapon = null;
        ProjectileWeapon = null;
        activeWeaponType = WeaponType.None;
    }


    private void AttackByType()
    {
        switch (activeWeaponType)
        {
            case WeaponType.Melee:
                if (Input.GetMouseButtonDown(0) && MeleeWeapon != null)
                {
                    //MeleeWeapon.PrimaryAttack();
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
                if (ProjectileWeapon != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProjectileWeapon.PrimaryAttack();
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        ProjectileWeapon.ReleasePullback();
                    }
                }
                break;

            case WeaponType.None:
                // No weapon equipped, no attack possible
                break;
        }
    }
}
