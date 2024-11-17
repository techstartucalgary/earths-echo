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

        //change RB stats based on projectile type
        SetRBStats();
        InitializeProjectileStats();
    }

    private void FixedUpdate()
    {
        if (projectileType == ProjectileType.Physics)
        {
            transform.right = rb.velocity;
        }
    }
    private void SetRBStats()
    {
        if (projectileType == ProjectileType.Normal)
            rb.gravityScale = 0f;
        else if(projectileType == ProjectileType.Physics)
            rb.gravityScale = gravityScale;
    }
    private void InitializeProjectileStats()
    {
        if (projectileType == ProjectileType.Normal)
            SetStraightVelocity();
        else if (projectileType == ProjectileType.Physics)
            SetPhysicsVelocity();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((whatDestroysProjectile.value & (1 << collision.gameObject.layer)) > 0)
        {
            // spawn effects
            // play sound
            // shake screen
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.Damage(damage);
            }
            Debug.Log($"Hit object with layer: {LayerMask.LayerToName(collision.gameObject.layer)}");

            Destroy(gameObject);
        }
    }

    private void SetPhysicsVelocity()
    {
        rb.velocity = transform.right * physicsProjectileVelocity;
    }
    private void SetStraightVelocity()
    {
        rb.velocity = transform.right * normalProjectileVelocity;
    }

    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }
}
