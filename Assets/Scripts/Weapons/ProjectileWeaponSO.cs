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
    
    [Tooltip("The amount of damage dealt by this projectile weapon.")]
    public float damage = 3f;
}
