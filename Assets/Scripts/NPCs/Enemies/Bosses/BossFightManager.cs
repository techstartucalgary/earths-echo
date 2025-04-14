using UnityEngine;
using System.Collections;

public class BossFightManager : MonoBehaviour
{
    // Define boss phases: now only Phase1 and Phase2.
    public enum BossPhase { Phase1, Phase2 }
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Boss Components")]
    public TestEnemy bossHealth;        // Reference to the boss's health script.
    public TigerBossAttack enemyAttack;     // Reference to the EnemyAttack component.
    public Animator bossAnimator;       // Reference to the boss Animator.

    [Header("Phase Transition Settings")]
    [Tooltip("Duration in seconds for the revival/heal phase when transitioning from Phase1 to Phase2.")]
    public float phaseTransitionDuration = 5f;
    
    // Optional: Increase speed and damage in phase two.
    [Header("Phase Two Settings")]
    [Tooltip("Attack interval in Phase2 (faster than Phase1).")]
    public float phase2AttackInterval = 1.5f;
    [Tooltip("Damage multiplier in Phase2.")]
    public float phase2DamageMultiplier = 1.5f;

    [Header("Attack Settings")]
    public float attackInterval = 3f;   // Time between attacks (for Phase1).
    private float attackTimer = 0f;

    private void Start()
    {
        attackTimer = attackInterval;
    }

    private void Update()
    {
        // Check for phase transition based on defeat in Phase1.
        UpdatePhase();

        // Use the appropriate attack interval based on phase.
        float currentInterval = (currentPhase == BossPhase.Phase1) ? attackInterval : phase2AttackInterval;
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            PerformAttackPattern();
            attackTimer = currentInterval;
        }
    }

    // Update the boss phase based on current health.
    void UpdatePhase()
    {
        // When the boss is defeated in Phase1, trigger revival.
        if (bossHealth.currentHealth <= 0 && currentPhase == BossPhase.Phase1)
        {
            currentPhase = BossPhase.Phase2;
            bossAnimator.SetTrigger("Phase2");
            StartCoroutine(RevivalPhase());
        }
    }

    // Coroutine to handle the revival (healing) phase.
    IEnumerator RevivalPhase()
    {
        // Set boss invincible while healing.
        bossHealth.SetInvincible(true);

        // Calculate amount to heal (to full).
        float missingHealth = bossHealth.getMaxHealth() - bossHealth.currentHealth;
        float timer = 0f;

        while (timer < phaseTransitionDuration)
        {
            float healThisFrame = (missingHealth / phaseTransitionDuration) * Time.deltaTime;
            bossHealth.Heal(healThisFrame);
            timer += Time.deltaTime;
            yield return null;
        }

        bossHealth.SetInvincible(false);
    }

    // Execute different attack patterns based on the current phase.
    void PerformAttackPattern()
    {
        // Example: switch between different attack patterns.
        // You can integrate the new ground smash attack and other moves here.
        // For instance, randomly trigger ground smash, side attack, or upward attack.
        int attackChoice = Random.Range(0, 3);
        switch (attackChoice)
        {
            case 0:
                bossAnimator.SetTrigger("SideAttack");
                enemyAttack.triggerSideAttack = true;
                break;
            case 1:
                bossAnimator.SetTrigger("UpwardAttack");
                enemyAttack.triggerUpwardAttack = true;
                break;
            case 2:
                bossAnimator.SetTrigger("GroundSmash");
                enemyAttack.triggerGroundSmashAttack = true; // see note below
                break;
        }

        // Optionally, play an attack sound here.
        // SoundFXManager.Instance.PlayRandomSoundFXClip(new AudioClip[] { /* attack sound clip */ }, transform, 1f);
    }
}
