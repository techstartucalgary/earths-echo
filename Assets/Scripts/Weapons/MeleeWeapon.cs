using UnityEngine;
using UnityEngine.UI; // Required for UI components
using TMPro;
using System.Collections;

public class MeleeWeapon : Weapon
{
    public float swingSpeed; // Speed of the melee attack
    public float attackRange = 1.5f; // Range of the circular swing attack
    private BoxCollider2D weaponCollider; // The weapon's own collider
    private float lastSwingTime; // Time of the last swing
    public TMP_Text cooldownText; // Reference to the cooldown text UI element
    private GameObject player;
    public LayerMask enemies;

    public Transform attackPoint; // The point from which attacks originate

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

        // Initialize player reference
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found. Make sure there is a GameObject tagged 'Player' in the scene.");
        }
        attackPoint = player.transform.Find("AttackPoint"); // Assumes a child object named "AttackPoint"

        // Ensure attackPoint is assigned
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint not assigned. Make sure to set an attackPoint in the inspector.");
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

        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }
    }

    private void PerformCircularAttack(Vector2 attackDirection)
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is not set. Cannot perform attack.");
            return;
        }

        // Calculate attack position using attackPoint
        Vector2 attackPosition = (Vector2)attackPoint.transform.position + attackDirection * attackRange;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemies);

        foreach (var enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name);
            // Add logic to deal damage to the enemy here
        }
    }

    public override void PrimaryAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee primary attack with " + weaponName);
            PerformCircularAttack(Vector2.right); // Example direction for primary attack
        }
    }

    public override void SideAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;

            // Determine the direction based on the player's facing direction
            Vector2 attackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            Debug.Log("Melee side attack with " + weaponName);
            PerformCircularAttack(attackDirection); // Right or left direction for side attack
            Debug.Log("Performed circular attack " + (attackDirection == Vector2.right ? "right" : "left"));
        }
    }

    public override void UpAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee up attack with " + weaponName);

            // Perform an upward attack using a rectangular hitbox
            PerformVerticalAttack(Vector2.up);
        }
    }

    public override void DownAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee down attack with " + weaponName);

            // Perform a downward attack using a rectangular hitbox
            PerformVerticalAttack(Vector2.down);
        }
    }

    private void PerformVerticalAttack(Vector2 attackDirection)
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is not set. Cannot perform attack.");
            return;
        }

        // Define the size of the vertical hitbox
        Vector2 boxSize = new Vector2(0.5f, attackRange); // Narrow width, tall height

        // Calculate the center of the hitbox based on the attack direction
        Vector2 attackPosition = (Vector2)attackPoint.position + attackDirection * (attackRange / 2f);

        // Detect enemies in the rectangular area
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, boxSize, 0f, enemies);

        foreach (var enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name);
            // Add logic to deal damage to the enemy here
        }

        // Optional: Visualize the hitbox in debug mode
        Debug.DrawLine(attackPoint.position, attackPosition, Color.red, 0.5f);
    }


    private void OnDrawGizmosSelected()
    {
        if (weaponCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(weaponCollider.bounds.center, weaponCollider.bounds.size);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
            // Visualize the upward attack box
            Vector2 upAttackPosition = (Vector2)attackPoint.position + Vector2.up * (attackRange / 2f);
            Gizmos.DrawWireCube(upAttackPosition, new Vector2(0.5f, attackRange));

            // Visualize the downward attack box
            Vector2 downAttackPosition = (Vector2)attackPoint.position + Vector2.down * (attackRange / 2f);
            Gizmos.DrawWireCube(downAttackPosition, new Vector2(0.5f, attackRange));
        }
    }
}
