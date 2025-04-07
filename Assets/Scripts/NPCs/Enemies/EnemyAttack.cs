using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Points")]
    [SerializeField] protected Transform sideHitPoint;
    [SerializeField] protected Transform upwardHitPoint;
    [SerializeField] protected Transform downwardHitPoint;

    [Header("Attack Triggers")]
    // These booleans trigger an attack from the corresponding attack point.
    public bool triggerSideAttack = false;
    public bool triggerUpwardAttack = false;
    public bool triggerDownwardAttack = false;

    [Header("Attack Settings")]
    [SerializeField] protected float attackDamage = 5f;        // Damage inflicted per attack
    [SerializeField] protected float attackRange = 1f;         // Radius of each attack point
    [SerializeField] protected float attackCooldown = 1.5f;    // Time between consecutive attacks

    [Header("Target Information")]
    [SerializeField] protected LayerMask targetLayer;          // Layer of valid targets (e.g., the player)

    [Header("Attack Effects")]
    [SerializeField] protected AudioClip attackSound;          // Sound effect to play when attacking
    [SerializeField] protected float knockbackForce = 2f;        // Knockback force applied to targets

    private float nextAttackTime = 0f;

    protected virtual void Update()
    {
        // Only allow attacks if the cooldown period has passed.
        if (Time.time < nextAttackTime)
            return;

        bool attackPerformed = false;

        // Check each attack trigger and perform the attack if the boolean is set.
        if (triggerSideAttack)
        {
            attackPerformed |= AttackFromPoint(sideHitPoint);
            triggerSideAttack = false;
        }
        if (triggerUpwardAttack)
        {
            attackPerformed |= AttackFromPoint(upwardHitPoint);
            triggerUpwardAttack = false;
        }
        if (triggerDownwardAttack)
        {
            attackPerformed |= AttackFromPoint(downwardHitPoint);
            triggerDownwardAttack = false;
        }

        // If any attack was performed, set the cooldown and play the attack sound.
        if (attackPerformed)
        {
            nextAttackTime = Time.time + attackCooldown;
            if (attackSound != null)
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position);
            }
        }
    }

    // Attempts an attack from the specified hit point.
    // Returns true if at least one target was hit.
    private bool AttackFromPoint(Transform hitPoint)
    {
        if (hitPoint == null)
            return false;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, targetLayer);
        if (hitTargets.Length > 0)
        {
            foreach (Collider2D target in hitTargets)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    // Calculate the knockback direction from the hit point to the target.
                    Vector2 attackDirection = (target.transform.position - hitPoint.position).normalized;
                    damageable.Damage(attackDamage, attackDirection * knockbackForce);
                }
            }
            return true;
        }
        return false;
    }

    // Visualize the attack ranges for each attack point in the Unity Editor.
    protected virtual void OnDrawGizmosSelected()
    {
        if (sideHitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(sideHitPoint.position, attackRange);
        }
        if (upwardHitPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(upwardHitPoint.position, attackRange);
        }
        if (downwardHitPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(downwardHitPoint.position, attackRange);
        }
    }
}
