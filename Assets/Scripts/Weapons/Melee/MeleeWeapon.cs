using UnityEngine;
using TMPro;

public class MeleeWeapon : Weapon
{
    public float swingSpeed; // Cooldown between swings
    private BoxCollider2D weaponCollider; // Collider for the weapon
    private float lastSwingTime; // Time of the last swing
    public TMP_Text cooldownText; // UI element to show cooldown
    public LayerMask enemies; // LayerMask to identify enemies
    public float damage; // Amount of damage dealt
    public float range; // Range of the melee attack

    public Transform attackPoint; // Point where the attack originates
    //private Animator animator; // Animator for melee animations

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider2D>();
        if (weaponCollider == null)
        {
            Debug.LogError("Weapon collider not found. Adding automatically.");
            weaponCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        cooldownText = GameObject.Find("CooldownText")?.GetComponent<TMP_Text>();
        if (cooldownText == null)
        {
            Debug.LogError("CooldownText not found. Ensure a TextMeshPro UI element named 'CooldownText' exists.");
        }

        /*animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found. Animations will not be triggered.");
        }*/

        attackPoint = transform.Find("AttackPoint");
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint not assigned. Set an attackPoint in the inspector.");
        }
    }

    private void Update()
    {
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
        if (attackPoint == null) return;

        // Calculate the attack position
        Vector2 attackPosition = (Vector2)attackPoint.position + attackDirection * range;

        // Detect all enemies in the attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, range, enemies);

        foreach (var enemy in hitEnemies)
        {
            ApplyDamage(enemy);
        }
    }

    public override void PrimaryAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee primary attack with " + weaponName);

            // Trigger animation
            //animator?.SetTrigger("PrimaryAttack");

            PerformCircularAttack(Vector2.right);
        }
    }

    public override void SideAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;

            Vector2 attackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Debug.Log("Melee side attack with " + weaponName);

            //animator?.SetTrigger("SideAttack");

            PerformCircularAttack(attackDirection);
        }
    }

    public override void UpAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee up attack with " + weaponName);

            //animator?.SetTrigger("UpAttack");

            PerformVerticalAttack(Vector2.up);
        }
    }

    public override void DownAttack()
    {
        if (Time.time >= lastSwingTime + swingSpeed)
        {
            lastSwingTime = Time.time;
            Debug.Log("Melee down attack with " + weaponName);

            //animator?.SetTrigger("DownAttack");

            PerformVerticalAttack(Vector2.down);
        }
    }

    private void PerformVerticalAttack(Vector2 attackDirection)
    {
        if (attackPoint == null) return;

        // Define the size of the vertical hitbox
        Vector2 boxSize = new Vector2(0.5f, range);
        Vector2 attackPosition = (Vector2)attackPoint.position + attackDirection * (range / 2f);

        // Detect enemies in the rectangular area
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, boxSize, 0f, enemies);

        foreach (var enemy in hitEnemies)
        {
            ApplyDamage(enemy);
        }
    }

    private void ApplyDamage(Collider2D enemy)
    {
        // Apply damage if the enemy implements IDamageable
        IDamageable iDamageable = enemy.GetComponent<IDamageable>();
        if (iDamageable != null)
        {
            iDamageable.Damage(damage);
            Debug.Log($"Dealt {damage} damage to {enemy.name}");
        }
        else
        {
            Debug.Log($"{enemy.name} does not implement IDamageable.");
        }
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
            Gizmos.DrawWireSphere(attackPoint.position, range);
        }
    }
}
