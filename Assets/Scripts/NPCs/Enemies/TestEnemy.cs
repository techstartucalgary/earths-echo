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

    FindGrandchildren finder;
    public float currentHealth;
    public bool isInvincible = false;

    private Transform UpwardHitpoint;

    private Transform DownwardHitpoint;
    private Transform SideHitpoint;
    private float SideHitpointLocalX;
    private Vector3 previousScale;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform animatorTransform;
    private float animatorXScale;


    private void Start()
    {
        finder = new FindGrandchildren();
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.Initialize(maxHealth, enemyName);

        StartCoroutine(PlayIdleSounds());
        StartCoroutine(HealOverTime());

        UpwardHitpoint = finder.FindDeepChild(transform, "UpwardHitpoint");
        DownwardHitpoint = finder.FindDeepChild(transform, "DownwardHitpoint");
        SideHitpoint = finder.FindDeepChild(transform, "SideHitpoint");

        if (SideHitpoint != null)
            SideHitpointLocalX = SideHitpoint.localPosition.x;

        previousScale = transform.localScale; // Initialize previous scale
        animatorXScale = animatorTransform.localScale.x;
    
    }

    private void Update()
    {
        HandleHitpointFlip();
    }

    private void HandleHitpointFlip()
    {
        // Assuming the enemy flips the same way as the player via animatorTransform.localScale.x
        if (animatorTransform.localScale.x >= 0.01f)
        {
            // Facing right
            if(SideHitpoint)
                SideHitpoint.localPosition = new Vector3(SideHitpointLocalX, SideHitpoint.localPosition.y, SideHitpoint.localPosition.z);
        }
        else if (animatorTransform.localScale.x <= -0.01f)
        {
            // Facing left
            SideHitpoint.localPosition = new Vector3(-SideHitpointLocalX, SideHitpoint.localPosition.y, SideHitpoint.localPosition.z);
        }
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

    public float getMaxHealth(){
        return maxHealth;
    }
}
