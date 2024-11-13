using UnityEngine;
using UnityEngine.UI; // Required for UI components
using TMPro;

public class MeleeWeapon : Weapon
{
    public float swingSpeed; // Speed of the melee attack
    private BoxCollider2D weaponCollider; // The weapon's own collider
    private float lastSwingTime; // Time of the last swing
    public TMP_Text cooldownText; // Reference to the cooldown text UI element

    private void Start()
    {
        // Get the BoxCollider2D attached to this weapon GameObject
        weaponCollider = GetComponent<BoxCollider2D>();

        if (weaponCollider == null)
        {
            Debug.LogError("Weapon collider not found. Make sure weapon prefab has a BoxCollider component.");
            weaponCollider = gameObject.AddComponent<BoxCollider2D>();
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
            float cooldownRemaining = Mathf.Max(0f, (lastSwingTime + swingSpeed) - Time.time);
            cooldownText.text = $"Cooldown: {cooldownRemaining:F1} s";
        }
    }

    private void coolDown()
    {

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

    private void PerformAttack()
    {
        EnableCollider();

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            weaponCollider.bounds.center,
            weaponCollider.bounds.size,
            0f,
            LayerMask.GetMask("Enemy")
        );

        DisableCollider();
    }

    public override void PrimaryAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee primary attack with " + weaponName);
            PerformAttack();
        }
    }

    public override void SideAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee side attack with " + weaponName);
            PerformAttack();
        }
    }

    public override void UpAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee up attack with " + weaponName);
            PerformAttack();
        }
    }

    public override void DownAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee down attack with " + weaponName);
            PerformAttack();
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
