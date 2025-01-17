using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable, IHealable
{
    public EnemyHealthBar healthBar; // Optional health bar

    [SerializeField] private string enemyName; // Optional enemy name
    [SerializeField] private float maxHealth = 20f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health bar only if it is assigned
        if (healthBar != null)
        {
            // Pass the enemy name only if it's set
            healthBar.Initialize(maxHealth, enemyName);
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

    public void Heal(float healAmount)
    {
        //placeholder, will need further implementation with any healing items, schedules for enemies. 
    }
}
