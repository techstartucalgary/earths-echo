using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float swingSpeed; // Speed of the melee attack
    private BoxCollider2D weaponCollider; // The weapon's own collider
    private float lastSwingTime; // Time of the last swing

    private void Start()
    {
        // Get the BoxCollider2D attached to this weapon GameObject
        // Attempt to get the BoxCollider component
        weaponCollider = GetComponent<BoxCollider2D>();
        
        if (weaponCollider == null)
        {
            Debug.LogError("Weapon collider not found. Make sure weapon prefab has a BoxCollider component.");
            // Optional: Automatically add a BoxCollider if missing
            weaponCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("BoxCollider added automatically to the weapon.");
        }
    }

    private void EnableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    private void DisableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }
/*
    private void PerformAttack()
    {
        EnableCollider();
        
        // Detect enemies within the collider area
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            weaponCollider.bounds.center,
            weaponCollider.bounds.size,
            0f,
            LayerMask.GetMask("Enemy") // Adjust to match your enemy layer name
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            // Assuming enemies have a script with a TakeDamage method
            enemy.GetComponent<Enemy>()?.TakeDamage(damage);
            Debug.Log("Hit " + enemy.name + " with " + weaponName);
        }

        DisableCollider();
    }
*/
    public override void PrimaryAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee primary attack with " + weaponName);
            //PerformAttack();
            // Optional: Trigger animation here
        }
    }

    public override void SideAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee side attack with " + weaponName);
            //PerformAttack();
            // Optional: Rotate or adjust position to simulate side attack
        }
    }

    public override void UpAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee up attack with " + weaponName);
            //PerformAttack();
            // Optional: Adjust collider or animation for up attack
        }
    }

    public override void DownAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee down attack with " + weaponName);
            //PerformAttack();
            // Optional: Adjust collider or animation for down attack
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (weaponCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(weaponCollider.bounds.center, weaponCollider.bounds.size);
        }
    }
}
