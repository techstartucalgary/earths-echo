using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable, IHealable
{
    public EnemyHealthBar healthBar; // Optional health bar

    [SerializeField] private string enemyName; // Optional enemy name
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private AudioClip[] damageSoundClips;
    [SerializeField] private AudioClip[] idleSoundClips;
    [SerializeField] private float idleSoundInterval = 5f; // Time in seconds between idle sounds


    private float currentHealth;

    public void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health bar only if it is assigned
        if (healthBar != null)
        {
            // Pass the enemy name only if it's set
            healthBar.Initialize(maxHealth, enemyName);
        }
        StartCoroutine(PlayIdleSounds());

    }
    private IEnumerator PlayIdleSounds()
    {
        while (true)
        {
            // Play a random idle sound using the SoundFXManager
            SoundFXManager.Instance.PlayRandomSoundFXClip(idleSoundClips, transform, 1f);
            yield return new WaitForSeconds(idleSoundInterval);
        }
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Update the health bar only if it is assigned
        if (healthBar != null)
        {
            healthBar.Damage(damageAmount);
            //SoundFXManager.Instance.PlaySoundFXClip(damageSoundClips, transform, 1f);
            SoundFXManager.Instance.PlayRandomSoundFXClip(damageSoundClips, transform, 1f);
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
