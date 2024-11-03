using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float swingSpeed; // Speed of the melee attack
    private BoxCollider weaponCollider; // Declare weaponCollider
    private float lastSwingTime; // Time of the last swing

    private void Start()
    {
        // Assuming instantiatedWeapon is set when equipped
        if (instantiatedWeapon != null)
        {
            weaponCollider = instantiatedWeapon.GetComponent<BoxCollider>();
            if (weaponCollider == null)
            {
                Debug.LogError("Weapon collider not found. Make sure weapon prefab has a BoxCollider component.");
            }
            else
            {
                weaponCollider.isTrigger = true; // Set collider as a trigger
                weaponCollider.enabled = false; // Disable by default
            }
        }
    }

    private void EnableCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    private void DisableCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    private void PerformAttack()
    {
        EnableCollider();
        Collider[] hitEnemies = Physics.OverlapBox(
            weaponCollider.bounds.center,
            weaponCollider.bounds.extents,
            Quaternion.identity,
            LayerMask.GetMask("Enemy") // Adjust to your enemy layer name
        );

        // Commented out for future implementation
        /*
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(damage); // Assuming Enemy script has TakeDamage method
            Debug.Log("Hit " + enemy.name + " with " + weaponName);
        }
        */

        DisableCollider();
    }

    public override void PrimaryAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee primary attack with " + weaponName);
            PerformAttack();
            // Optional: Trigger animation here
        }
    }

    public override void SideAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee side attack with " + weaponName);
            PerformAttack();
            // Optional: Rotate or adjust position to simulate side attack
        }
    }

    public override void UpAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee up attack with " + weaponName);
            PerformAttack();
            // Optional: Adjust collider or animation for up attack
        }
    }

    public override void DownAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee down attack with " + weaponName);
            PerformAttack();
            // Optional: Adjust collider or animation for down attack
        }
    }
}
