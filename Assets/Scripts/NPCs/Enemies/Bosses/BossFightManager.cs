using UnityEngine;
using System.Collections;

public class BossFightManager : MonoBehaviour
{
    public enum BossPhase { Phase1, Phase2 }
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Boss Components")]
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
    
	[Header("Healing VFX")]
	[Tooltip("One‑shot burst when the boss begins healing.")]
	public GameObject healingStartPrefab;

	[Tooltip("Looping aura that stays on the boss while it heals.")]
	public GameObject healingLoopPrefab;

	private GameObject healingLoopInstance;   // runtime handle


    private void Start()
    {
        bossAnimator.SetTrigger("Intro");
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
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

            bossAnimator.SetTrigger("PhaseTransition");
            SoundFXManager.Instance.PlaySoundFXClip(phaseTransitionSound, transform, 1f);
            enemyAttack.isPhaseTwo = true;

            StartCoroutine(RevivalPhase());
        }
    }


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

        }

        float missingHealth = bossHealth.getMaxHealth() - bossHealth.currentHealth;
        float timer = 0f;
        while (timer < phaseTransitionDuration)
        {

            enemyAttack.animator.Play("tigerAnimation");

            float healThisFrame = (missingHealth / phaseTransitionDuration) * Time.deltaTime;
            bossHealth.Heal(healThisFrame);
            timer += Time.deltaTime;
            yield return null;
        }


        // Re-enable attacks and EnemyAI once healing is complete.

        enemyAttack.isHealing = false;
        bossHealth.SetInvincible(false);

        if (enemyAI != null)

            enemyAI.enabled = true;
    }
}

