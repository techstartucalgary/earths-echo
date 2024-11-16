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
        if (activeWeaponType == weaponType)
        {
            Debug.Log($"Weapon type {weaponType} is already active.");
            return;
        }

        // Destroy currently active weapon
        UnequipWeapon();

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

            Debug.Log($"Activated {weaponType.ToString().ToLower()} weapon: {activeWeaponInstance.name}");
            activeWeaponType = weaponType;
        }
        else
        {
            Debug.LogWarning($"No weapon prefab set for {weaponType.ToString().ToLower()} weapon.");
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
                // No weapon equipped, no attack possible
                break;
        }
    }
}
