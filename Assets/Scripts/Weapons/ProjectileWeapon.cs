using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public GameObject projectilePrefab; // The prefab for the projectile to shoot
    public Transform firePoint; // Where the projectile will spawn

    public override void PrimaryAttack()
    {
        Debug.Log(weaponName + " primary projectile attack!");
        ShootProjectile();
    }

    public override void SideAttack()
    {
        Debug.Log(weaponName + " side projectile attack!");
        ShootProjectile();
    }

    public override void UpAttack()
    {
        Debug.Log(weaponName + " up projectile attack!");
        ShootProjectile();
    }

    public override void DownAttack()
    {
        Debug.Log(weaponName + " down projectile attack!");
        ShootProjectile();
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Debug.Log("Projectile fired!");
        }
        else
        {
            Debug.LogWarning("Projectile prefab or fire point not set.");
        }
    }
}
