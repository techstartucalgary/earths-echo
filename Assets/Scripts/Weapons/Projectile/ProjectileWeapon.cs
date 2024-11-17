using UnityEngine;
using UnityEngine.UI; // Required for UI components
using TMPro;
using System.Collections;

public class ProjectileWeapon : Weapon
{
    public GameObject projectilePrefab; // The prefab for the projectile to shoot
    public Transform firePoint; // Where the projectile will spawn
    public float shootDelay;
    private float lastShootDelay;
    private BoxCollider2D boxCollider;
    public TMP_Text cooldownText; // Reference to the cooldown text UI element


    void Start()
    {
        // Attempt to get the BoxCollider component
        boxCollider = GetComponent<BoxCollider2D>();
        
        if (boxCollider == null)
        {
            Debug.LogError("Weapon collider not found. Make sure weapon prefab has a BoxCollider component.");
            // Optional: Automatically add a BoxCollider if missing
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("BoxCollider added automatically to the weapon.");
        }
        // Check if cooldownText is assigned
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
        // Update the cooldown text
        if (cooldownText != null)
        {
            float cooldownRemaining = Mathf.Max(0f, (lastShootDelay + shootDelay) - Time.time);
            cooldownText.text = $"Cooldown: {cooldownRemaining:F1} s";
        }

        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }
    }
    public override void PrimaryAttack()
    {
        if (Time.time >= lastShootDelay + shootDelay) {
            lastShootDelay = Time.time;
            Debug.Log(weaponName + " primary projectile attack!");
            ShootProjectile();
        }
            
    }

    public override void SideAttack()
    {
        Debug.Log(weaponName + " side projectile attack!");
 
    }

    public override void UpAttack()
    {
        Debug.Log(weaponName + " up projectile attack!");

    }

    public override void DownAttack()
    {
        Debug.Log(weaponName + " down projectile attack!");
 
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
