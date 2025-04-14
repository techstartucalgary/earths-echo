using UnityEngine;
using System.Collections;

public class BossFightManager : MonoBehaviour
{
    public enum BossPhase { Phase1, Phase2 }
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Boss Components")]
    public TestEnemy bossHealth;          // Boss health script.
    public TigerBossAttack enemyAttack;   // Boss attack/state machine.
    public Animator bossAnimator;         // Boss Animator.

    [Header("Phase Transition Settings")]
    [Tooltip("Duration (in seconds) for the revival/heal phase when transitioning from Phase1 to Phase2.")]
    public float phaseTransitionDuration = 5f;
    public AudioClip phaseTransitionSound;
    
    [Header("Phase Two Settings")]
    [Tooltip("Damage multiplier in Phase2.")]
    public float phase2DamageMultiplier = 1.5f;

    private EnemyAI enemyAI;

    private void Start()
    {
        // Trigger the intro animation.
        bossAnimator.SetTrigger("Intro");

        // Get the EnemyAI component reference.
        enemyAI = GetComponent<EnemyAI>();
        AudioManager.instance.SetGameplayMusic(GameplayContext.TigerBossFight);
    }

    private void Update()
    {
        UpdatePhase();
    }

    /// <summary>
    /// Checks if the boss's health has dropped below 25% during Phase1.
    /// </summary>
    void UpdatePhase()
    {
        if (currentPhase == BossPhase.Phase1 && bossHealth.currentHealth <= bossHealth.getMaxHealth() * 0.25f)
        {
            currentPhase = BossPhase.Phase2;
            bossAnimator.SetTrigger("PhaseTransition"); // Phase transition animation.
            // Optionally, play a sound cue here.
            SoundFXManager.Instance.PlaySoundFXClip(phaseTransitionSound, transform, 1f);
            enemyAttack.isPhaseTwo = true;  // Inform the attack state machine that we are now in Phase2.
            StartCoroutine(RevivalPhase());
        }
    }

    /// <summary>
    /// Heals the boss over the transition duration while making it invincible and disabling its attacks.
    /// </summary>
    IEnumerator RevivalPhase()
    {
        // Set boss invincible and disable attack logic.
        bossHealth.SetInvincible(true);
        enemyAttack.isHealing = true;

        // Disable the EnemyAI component so the boss stops moving.
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        float missingHealth = bossHealth.getMaxHealth() - bossHealth.currentHealth;
        float timer = 0f;
        while (timer < phaseTransitionDuration)
        {
            float healThisFrame = (missingHealth / phaseTransitionDuration) * Time.deltaTime;
            bossHealth.Heal(healThisFrame);
            timer += Time.deltaTime;
            yield return null;
        }

        // Re-enable attacks and EnemyAI once healing is complete.
        enemyAttack.isHealing = false;
        bossHealth.SetInvincible(false);

        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }
    }
}


