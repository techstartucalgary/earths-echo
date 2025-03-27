using UnityEngine;

public abstract class WeaponSO : ItemSO
{
    [Header("Weapon Visuals")]
    [Tooltip("The sprite representing the weapon.")]
    public Sprite weaponSprite;

    [Header("Weapon Stats")]
    [Tooltip("The cooldown time between attacks.")]
    public float cooldownTime = 0.5f;
    
    [Tooltip("The range of the weapon's attack.")]
    public float range = 0.5f;

	public float damage = 5f;


    private void OnValidate()
    {
        // Weapons should not be stackable.
        stackable = false;
        usable = false;
        maxStack = 1;
    }
}
