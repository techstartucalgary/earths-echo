using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileWeapon", menuName = "Weapon/Projectile Weapon")]
public class ProjectileWeaponSO : WeaponSO
{
    [Header("Projectile Specific Stats")]
    [Tooltip("The prefab for the projectile.")]
    public GameObject projectilePrefab;
    public GameObject secondaryProjectilePrefab;

    [Tooltip("The speed at which the projectile moves.")]
    public float projectileSpeed = 10f;
    
}
