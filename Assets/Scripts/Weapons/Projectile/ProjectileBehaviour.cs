using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [Header("General Projectile Stats")]
    [SerializeField] private float destroyTime;
    [SerializeField] private LayerMask whatDestroysProjectile;
    [SerializeField] private float damage;

    [Header("Normal Projectile Stats")]
    [SerializeField] private float normalProjectileVelocity;

    [Header("Physics Projectile Stats")]
    [SerializeField] private float physicsProjectileVelocity;
    [SerializeField] private float gravityScale;

    private Rigidbody2D rb;

    public enum ProjectileType
    {
        Normal,
        Physics
    }
    public ProjectileType projectileType;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on the projectile!");
            return;
        }

        SetDestroyTime();
        InitializeProjectile();
    }

    private void FixedUpdate()
    {
        // Rotate the projectile to align with its velocity for Physics type
        if (projectileType == ProjectileType.Physics)
        {
            transform.right = rb.velocity.normalized;
        }
    }

    private void InitializeProjectile()
    {
        // Configure Rigidbody based on projectile type
        if (projectileType == ProjectileType.Normal)
        {
            rb.gravityScale = 0f;
            SetStraightVelocity();
        }
        else if (projectileType == ProjectileType.Physics)
        {
            rb.gravityScale = gravityScale;
            SetPhysicsVelocity();
        }
    }

    private void SetStraightVelocity()
    {
        rb.velocity = transform.right * normalProjectileVelocity;
    }

    private void SetPhysicsVelocity()
    {
        rb.velocity = transform.right * physicsProjectileVelocity;
    }

    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }

    public void AdjustGravityScale(float newGravityScale)
    {
        if (projectileType == ProjectileType.Physics)
        {
            gravityScale = newGravityScale;
            rb.gravityScale = gravityScale;

            Debug.Log($"Projectile gravity scale adjusted to: {gravityScale}");
        }
        else
        {
            Debug.LogWarning("Cannot adjust gravity scale for a non-Physics projectile.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((whatDestroysProjectile.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Deal damage if applicable
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.Damage(damage);
            }

            Debug.Log($"Hit object with layer: {LayerMask.LayerToName(collision.gameObject.layer)}");
            Destroy(gameObject);
        }
    }
}
