using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    public EnemyHealthBar healthBar; // Optional health bar

    [SerializeField] private float maxHealth = 20f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health bar only if it is assigned
        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth);
        }
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Update the health bar only if it is assigned
        if (healthBar != null)
        {
            healthBar.Damage(damageAmount);
        }

        // Destroy the enemy if health is zero or below
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
