using System.Collections;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    public EnemyHealthBar healthBar; // Optional health bar

    [SerializeField] private string enemyName; // Optional enemy name
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private AudioClip[] damageSoundClips;
    [SerializeField] private AudioClip[] idleSoundClips;
    [SerializeField] private AudioClip[] deathSoundClips;

    [SerializeField] private float idleSoundInterval = 5f; // Seconds between idle sounds

    // Reference to the ScreenShake script (assign this in the Inspector)
    [SerializeField] private ScreenShake screenShake;
    // Screen shake parameters
    [SerializeField] private float shakeDuration = 0.2f;
    private float shakeMagnitude = 0.05f;

    // Enemy healing settings:
    [SerializeField] private bool canHealEnemy = false;  // If true, the enemy heals over time.
    [SerializeField] private float enemyHealAmount = 5f;   // Amount healed each interval.
    [SerializeField] private float healInterval = 5f;      // Time in seconds between heals.

    public float currentHealth;
    public bool isInvincible = false;

    private void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health bar if assigned.
        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth, enemyName);
        }

        StartCoroutine(PlayIdleSounds());
        // Start the healing coroutine once. It will handle the healing logic internally.
        StartCoroutine(HealOverTime());
    }

    private IEnumerator PlayIdleSounds()
    {
        while (true)
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(idleSoundClips, transform, 1f);
            yield return new WaitForSeconds(idleSoundInterval);
        }
    }

    // Persistent coroutine that checks canHealEnemy before healing.
    private IEnumerator HealOverTime()
    {
        while (true)
        {
            // Only wait and heal if healing is enabled.
            if (canHealEnemy)
            {
                yield return new WaitForSeconds(healInterval);
                Heal(enemyHealAmount);
            }
            else
            {
                // If healing is not enabled, yield until next frame.
                yield return null;
            }
        }
    }

    public void Damage(float damageAmount,  Vector2 impactPos)
    {
        // If the enemy is invincible, ignore damage.
        if (isInvincible)
            return;

        currentHealth -= damageAmount;

        if (healthBar != null)
        {
            healthBar.Damage(damageAmount);
        }
        
        Vector3 knockbackDirection = impactPos;
        Debug.Log(knockbackDirection);
        float knockbackForce = damageAmount * 0.5f;
        ApplyKnockback(knockbackDirection, knockbackForce);

        SoundFXManager.Instance.PlayRandomSoundFXClip(damageSoundClips, transform, 1f);

        // Trigger screen shake effect.
        if (screenShake != null)
        {
            shakeDuration = damageAmount / 10;
            screenShake.Shake(shakeDuration, shakeMagnitude);
        }

        if (currentHealth <= 0)
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(deathSoundClips, transform, 1f);
            Destroy(gameObject); // will need to change this call to an animator that makes the enemy get knocked out. Might be useful to remove colliders atp
            //AudioManager.instance.SetGameplayMusic(GameplayContext._);
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.Heal(healAmount);
        }
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }
}
