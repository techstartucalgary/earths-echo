using UnityEngine;
using System.Collections;

public class BossFightManager : MonoBehaviour
{
    public enum BossPhase { Phase1, Phase2 }
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Boss Components")]
<<<<<<< HEAD
    public TestEnemy bossHealth;
    public TigerBossAttack enemyAttack;
    public Animator bossAnimator;

    [Header("Phase Transition Settings")]
    public float phaseTransitionDuration = 5f;
    public AudioClip phaseTransitionSound;

    [Header("Phase Two Settings")]
    public float phase2DamageMultiplier = 1.5f;

    [Header("Healing Settings")]
    [Tooltip("Position the boss jumps to during healing.")]
    public Transform healingSpot;

    private EnemyAI enemyAI;
    private Rigidbody2D rb;

    private void Start()
    {
        bossAnimator.SetTrigger("Intro");
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
=======
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
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
        AudioManager.instance.SetGameplayMusic(GameplayContext.TigerBossFight);
    }

    private void Update()
    {
        UpdatePhase();
    }

<<<<<<< HEAD
=======
    /// <summary>
    /// Checks if the boss's health has dropped below 25% during Phase1.
    /// </summary>
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
    void UpdatePhase()
    {
        if (currentPhase == BossPhase.Phase1 && bossHealth.currentHealth <= bossHealth.getMaxHealth() * 0.25f)
        {
            currentPhase = BossPhase.Phase2;
<<<<<<< HEAD
            bossAnimator.SetTrigger("PhaseTransition");
            SoundFXManager.Instance.PlaySoundFXClip(phaseTransitionSound, transform, 1f);
            enemyAttack.isPhaseTwo = true;
=======
            bossAnimator.SetTrigger("PhaseTransition"); // Phase transition animation.
            // Optionally, play a sound cue here.
            SoundFXManager.Instance.PlaySoundFXClip(phaseTransitionSound, transform, 1f);
            enemyAttack.isPhaseTwo = true;  // Inform the attack state machine that we are now in Phase2.
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
            StartCoroutine(RevivalPhase());
        }
    }

<<<<<<< HEAD
    IEnumerator RevivalPhase()
    {
        bossHealth.SetInvincible(true);
        enemyAttack.isHealing = true;

        if (enemyAI != null)
            enemyAI.enabled = false;

        // Move to healing spot
        if (healingSpot != null)
        {
            // Optional: leap with Rigidbody2D force
            if (rb != null)
            {
                Vector2 jumpForce = (healingSpot.position - transform.position) * 2f;
                rb.velocity = Vector2.zero; // reset current velocity
                rb.AddForce(jumpForce, ForceMode2D.Impulse);
                enemyAttack.animator.Play("tigerJump");
            }
            else
            {
                transform.position = healingSpot.position;
            }

            // Optional: wait a short moment for animation or travel
            yield return new WaitForSeconds(0.5f);
=======
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
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
        }

        float missingHealth = bossHealth.getMaxHealth() - bossHealth.currentHealth;
        float timer = 0f;
        while (timer < phaseTransitionDuration)
        {
<<<<<<< HEAD
            enemyAttack.animator.Play("tigerAnimation");
=======
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
            float healThisFrame = (missingHealth / phaseTransitionDuration) * Time.deltaTime;
            bossHealth.Heal(healThisFrame);
            timer += Time.deltaTime;
            yield return null;
        }

<<<<<<< HEAD
=======
        // Re-enable attacks and EnemyAI once healing is complete.
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
        enemyAttack.isHealing = false;
        bossHealth.SetInvincible(false);

        if (enemyAI != null)
<<<<<<< HEAD
            enemyAI.enabled = true;
    }
}
=======
        {
            enemyAI.enabled = true;
        }
    }
}


>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
