using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileWeapon : Weapon
{
    public GameObject projectilePrefab; // The prefab for the projectile to shoot
    public GameObject secondaryProjectilePrefab;
    private GameObject chosenPrefab;
    public Transform firePoint; // Where the projectile will spawn
    public float shootDelay;
    public float pullbackDuration; // Time to hold for pullback weapons
    private float lastShootDelay;
    private float pullbackStartTime;
    
    private bool isPullingBack;

    public TMP_Text cooldownText; // Reference to the cooldown text UI element

    public enum ProjectileMechanic
    {
        Instant,
        Pullback
    }
    public ProjectileMechanic projectileMechanic = ProjectileMechanic.Instant; // Default is instant firing

    private void Start()
    {
        var boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("Weapon collider not found. Make sure weapon prefab has a BoxCollider component.");
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("BoxCollider added automatically to the weapon.");
        }

        if (cooldownText == null)
        {
            cooldownText = GameObject.Find("CooldownText")?.GetComponent<TMP_Text>();
            if (cooldownText == null)
            {
                Debug.LogError("CooldownText TMP_Text component not found in the scene. Make sure there is a TextMeshPro UI element named 'CooldownText'.");
            }
        }
    }

    private void Update()
    {
        // Update the cooldown text dynamically
        if (cooldownText != null)
        {
            if (projectileMechanic == ProjectileMechanic.Pullback && isPullingBack)
            {
                float pullbackProgress = Mathf.Clamp01((Time.time - pullbackStartTime) / pullbackDuration);
                cooldownText.text = $"Pullback: {pullbackProgress * 100:F0}%";
            }
            else if (projectileMechanic == ProjectileMechanic.Instant || !isPullingBack)
            {
                float cooldownRemaining = Mathf.Max(0f, (lastShootDelay + shootDelay) - Time.time);
                cooldownText.text = $"Cooldown: {cooldownRemaining:F1} s";
            }
        }
    }

    public override void PrimaryAttack()
    {
        if (!GameManager.instance.CanProcessGameplayActions())
        {
            return;
        }
        chosenPrefab = projectilePrefab;
            if (projectileMechanic == ProjectileMechanic.Instant)
        {
            if (Time.time >= lastShootDelay + shootDelay)
            {
                lastShootDelay = Time.time;
                Debug.Log(weaponName + " primary projectile attack!");

                ShootProjectile(1f); // Fire at full power for instant mechanics
            }
        }
        else if (projectileMechanic == ProjectileMechanic.Pullback)
        {
            if (Time.time >= lastShootDelay + shootDelay)
            {
                isPullingBack = true;
                pullbackStartTime = Time.time;
                Debug.Log("Pullback started...");
            }
        }
    }

    public void ReleasePullback()
    {
        if (isPullingBack)
        {
            float pullbackProgress = Mathf.Clamp01((Time.time - pullbackStartTime) / pullbackDuration);

            if (pullbackProgress >= 0.1f) // Ensure a minimum pullback
            {
                isPullingBack = false;
                lastShootDelay = Time.time;
                Debug.Log("Pullback released, firing projectile!");
                ShootProjectile(pullbackProgress);
            }
            else
            {
                Debug.Log("Pullback cancelled or too short.");
                isPullingBack = false;
            }
        }
    }

    public void ShootProjectile(float powerPercentage)
    {
        if (chosenPrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(chosenPrefab, firePoint.position, firePoint.rotation);
            ProjectileBehaviour projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>();

            if (projectileBehaviour != null && projectileBehaviour.projectileType == ProjectileBehaviour.ProjectileType.Physics)
            {
                projectileBehaviour.AdjustDamageScale(powerPercentage);

                float adjustedGravityScale = Mathf.Lerp(5f, 0.5f, powerPercentage); // Adjust gravity based on pullback
                projectileBehaviour.AdjustGravityScale(adjustedGravityScale);
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab or fire point not set.");
        }
    }

    public override void SideAttack()
    {
        if (!GameManager.instance.CanProcessGameplayActions())
        {
            return;
        }
        chosenPrefab = secondaryProjectilePrefab;
            if (projectileMechanic == ProjectileMechanic.Instant)
        {
            if (Time.time >= lastShootDelay + shootDelay)
            {
                lastShootDelay = Time.time;
                Debug.Log(weaponName + " primary projectile attack!");

                ShootProjectile(1f); // Fire at full power for instant mechanics
            }
        }
        else if (projectileMechanic == ProjectileMechanic.Pullback)
        {
            if (Time.time >= lastShootDelay + shootDelay)
            {
                isPullingBack = true;
                pullbackStartTime = Time.time;
                Debug.Log("Pullback started...");
            }
        }
    }

    public override void UpAttack()
    {
        if (!GameManager.instance.CanProcessGameplayActions())
        {
            return;
        }
        Debug.Log(weaponName + " up projectile attack!");
    }

    public override void DownAttack()
    {
        if (!GameManager.instance.CanProcessGameplayActions())
        {
            return;
        }
        Debug.Log(weaponName + " down projectile attack!");
    }

    public float GetPullbackPower()
    {
        if (projectileMechanic == ProjectileMechanic.Pullback && isPullingBack)
        {
            return Mathf.Clamp01((Time.time - pullbackStartTime) / pullbackDuration);
        }

        return 1f; // Default to full power for instant mechanics
    }

}




