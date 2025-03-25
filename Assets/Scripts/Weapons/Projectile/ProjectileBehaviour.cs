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
    public float physicsProjectileVelocity;
    public float gravityScale;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb; // Make Rigidbody2D a serialized field

    public enum ProjectileType
    {
        Normal,
        Physics
    }
    public ProjectileType projectileType;

    private void Start()
    {
        // If rb is not assigned in the Inspector, try to fetch it dynamically
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError($"Rigidbody2D not assigned or found on the projectile '{gameObject.name}'.");
                return;
            }
        }

        SetDestroyTime();
        InitializeProjectile();
    }

    private void FixedUpdate()
    {
        if (projectileType == ProjectileType.Physics)
        {
            transform.right = rb.velocity.normalized;
        }
    }

    private void InitializeProjectile()
    {
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
        if (rb == null)
        {
            Debug.LogError($"Rigidbody2D is missing on projectile '{gameObject.name}'. Cannot adjust gravity scale.");
            return;
        }

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

    public void AdjustDamageScale(float newDamagePercentage)
    {
        if (projectileType == ProjectileType.Physics)
        {
            damage *= (2 * newDamagePercentage);
            Debug.Log($"Projectile damage adjusted to: {damage}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((whatDestroysProjectile.value & (1 << collision.gameObject.layer)) > 0)
        {
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.Damage(damage, rb.velocity);
            }

            Debug.Log($"Hit object with layer: {LayerMask.LayerToName(collision.gameObject.layer)}");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (rb == null) return; // Skip if Rigidbody2D is not assigned

        Gizmos.color = Color.red;
        Vector2 position = transform.position;
        Vector2 velocity = rb.velocity;
        for (int i = 0; i < 50; i++)
        {
            float time = i * Time.fixedDeltaTime;
            Vector2 gravityEffect = 0.5f * Physics2D.gravity * rb.gravityScale * time * time;
            Vector2 nextPosition = position + velocity * time + gravityEffect;

            Gizmos.DrawLine(position, nextPosition);
            position = nextPosition;
        }
    }
}
