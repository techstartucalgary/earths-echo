using UnityEngine;
using System.Collections;

public class BossFightManager : MonoBehaviour
{
    // Define boss phases.
    public enum BossPhase { Phase1, Phase2, Phase3 }
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Boss Components")]
    public TestEnemy bossHealth;        // Reference to the boss's health script.
    public EnemyAttack enemyAttack;     // Reference to the EnemyAttack component.
    public Animator bossAnimator;       // Reference to the boss Animator.

    [Header("Phase Thresholds")]
    [Tooltip("Boss will transition to Phase2 when health is below this percentage.")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.7f;  // 70% health.
    [Tooltip("Boss will transition to Phase3 when health is below this percentage.")]
    [Range(0f, 1f)]
    public float phase3Threshold = 0.4f;  // 40% health.

    [Header("Phase Transition Settings")]
    [Tooltip("Duration in seconds for the phase transition during which the boss is invincible and heals.")]
    public float phaseTransitionDuration = 5f;
    [Tooltip("Percentage of max health to heal at the start of a new phase (e.g., 0.1 for 10%).")]
    public float phaseHealPercentage = 0.1f;

    [Header("Attack Settings")]
    public float attackInterval = 3f;   // Time between attacks.
    private float attackTimer = 0f;

    private void Start()
    {
        attackTimer = attackInterval;
    }

    private void Update()
    {
        UpdatePhase();
        // Countdown for the next attack.
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            PerformAttackPattern();
            attackTimer = attackInterval;
        }
    }

    // Update the boss phase based on current health.
    void UpdatePhase()
    {
        // Calculate the percentage of health remaining.
        float healthPercent = bossHealth.currentHealth / bossHealth.getMaxHealth();

        // Transition to Phase3 if health is below phase3Threshold.
        if (healthPercent <= phase3Threshold && currentPhase != BossPhase.Phase3)
        {
            currentPhase = BossPhase.Phase3;
            bossAnimator.SetTrigger("Phase3");
            // Play a phase transition sound effect if desired.
            SoundFXManager.Instance.PlayRandomSoundFXClip(new AudioClip[] { /* add Phase3 audio clips */ }, transform, 1f);
            StartCoroutine(PhaseTransition());
        }
        // Transition to Phase2 if health is below phase2Threshold and still in Phase1.
        else if (healthPercent <= phase2Threshold && currentPhase == BossPhase.Phase1)
        {
            currentPhase = BossPhase.Phase2;
            bossAnimator.SetTrigger("Phase2");
            SoundFXManager.Instance.PlayRandomSoundFXClip(new AudioClip[] { /* add Phase2 audio clips */ }, transform, 1f);
            StartCoroutine(PhaseTransition());
        }
    }

    // Coroutine to manage phase transition invincibility and gradual healing.
    IEnumerator PhaseTransition()
    {
        // Make the boss invincible.
        bossHealth.SetInvincible(true);

        // Calculate the total healing amount (10% of max health).
        float totalHealAmount = bossHealth.getMaxHealth() * phaseHealPercentage;
        float healedSoFar = 0f;
        float timer = 0f;

        // Gradually heal over the duration.
        while (timer < phaseTransitionDuration)
        {
            float healThisFrame = (totalHealAmount / phaseTransitionDuration) * Time.deltaTime;
            bossHealth.Heal(healThisFrame);
            healedSoFar += healThisFrame;
            timer += Time.deltaTime;
            yield return null;
        }

        // End the phase transition period: remove invincibility.
        bossHealth.SetInvincible(false);
    }

    // Execute different attack patterns based on the current boss phase.
    void PerformAttackPattern()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                // Phase1: perform a side attack.
                bossAnimator.SetTrigger("SideAttack");
                enemyAttack.triggerSideAttack = true;
                break;
            case BossPhase.Phase2:
                // Phase2: alternate between side and upward attack.
                if (Random.Range(0, 2) == 0)
                {
                    bossAnimator.SetTrigger("SideAttack");
                    enemyAttack.triggerSideAttack = true;
                }
                else
                {
                    bossAnimator.SetTrigger("UpwardAttack");
                    enemyAttack.triggerUpwardAttack = true;
                }
                break;
            case BossPhase.Phase3:
                // Phase3: use a random mix of all three attack types.
                int attackChoice = Random.Range(0, 3);
                if (attackChoice == 0)
                {
                    bossAnimator.SetTrigger("SideAttack");
                    enemyAttack.triggerSideAttack = true;
                }
                else if (attackChoice == 1)
                {
                    bossAnimator.SetTrigger("UpwardAttack");
                    enemyAttack.triggerUpwardAttack = true;
                }
                else
                {
                    bossAnimator.SetTrigger("DownwardAttack");
                    enemyAttack.triggerDownwardAttack = true;
                }
                break;
        }

        // Optionally, play an attack sound here.
        // SoundFXManager.Instance.PlayRandomSoundFXClip(new AudioClip[] { /* attack sound clip */ }, transform, 1f);
    }
}
