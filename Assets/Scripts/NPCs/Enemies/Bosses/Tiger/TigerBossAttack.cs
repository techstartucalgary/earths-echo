using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TigerBossAttack : EnemyAttack
{
    [Header("ScriptableObject States")]
    public BossStateSO idleState;
    public BossStateSO rageState;      // Rage state (with sin-wave movement) lasts for a fixed duration.
    public BossStateSO clawState;
    public BossStateSO groundSmashState;

    [Header("Rage Settings")]
    [Tooltip("Current rage level.")]
    public float currentRage = 0f;
    [Tooltip("Rage meter maximum value.")]
    public float maxRage = 100f;
    [Tooltip("Rage increase per second.")]
    public float rageIncreaseRate = 10f;

    [Header("Rage State Settings")]
    [Tooltip("Duration (seconds) of the rage state.")]
    public float rageDuration = 20f;
    private float rageTimer = 0f;
    public bool isRaging = false;

    [Header("Physical Rage Bar")]
    [Tooltip("UI Slider representing the boss's rage bar.")]
    public Slider rageBar;

    [Header("References and Movement")]
    public Animator animator;     // Assign in Inspector.
    public EnemyAI enemyAI;       // Fallback via GetComponent.
    public float dashSpeedMultiplier = 2f; // Multiplier during rage state.

    // Existing hit points and other properties.
    public Transform clawHitPoint;
    public LayerMask TargetLayer => targetLayer;  // From base (assumed defined).
    public float KnockbackForce => knockbackForce;  // From base (assumed defined).

    // Store the starting x-position for sin wave movement.
    [HideInInspector] public float initialX;
    // Flag to pause movement and attack logic during healing.
    [HideInInspector] public bool isHealing = false;
    // Flag to indicate Phase2 (set externally by BossFightManager).
    [HideInInspector] public bool isPhaseTwo = false;

    private BossStateSO currentState;

    protected void Start()
    {
        if (enemyAI == null)
        {
            enemyAI = GetComponent<EnemyAI>();
        }
        initialX = transform.position.x;
        TransitionToState(idleState);
        UpdateRageUI();
    }

    protected override void Update()
    {
        // Skip attack/movement logic when healing.
        if (isHealing)
        {
            UpdateRageUI();
            return;
        }

        base.Update();

        // Accumulate rage only when not already raging.
        if (!isRaging)
        {
            IncreaseRage(rageIncreaseRate * Time.deltaTime);
        }
        else
        {
            rageTimer += Time.deltaTime;
            if (rageTimer >= rageDuration)
            {
                EndRageState();
            }
        }

        UpdateRageUI();

        if (currentRage >= maxRage && !isRaging)
        {
            ActivateRageState();
        }

        currentState?.UpdateState(this);
    }

    /// <summary>
    /// Transitions to a new state.
    /// </summary>
    public void TransitionToState(BossStateSO newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    public void IncreaseRage(float amount)
    {
        currentRage = Mathf.Clamp(currentRage + amount, 0, maxRage);
    }

    public void ActivateRageState()
    {
        isRaging = true;
        rageTimer = 0f;
        currentRage = maxRage;
        enemyAI.speed *= dashSpeedMultiplier;
        TransitionToState(rageState);
        Debug.Log("Rage state activated!");
    }

    public void EndRageState()
    {
        isRaging = false;
        rageTimer = 0f;
        enemyAI.speed /= dashSpeedMultiplier;
        currentRage = 0f;
        TransitionToState(idleState);
        Debug.Log("Rage state ended.");
    }

    private void UpdateRageUI()
    {
        if (rageBar != null)
        {
            rageBar.value = currentRage / maxRage;
        }
    }

    /// <summary>
    /// Chooses the next attack state when the boss is idle.
    /// For example, randomly choose between a Claw Attack and a Ground Smash.
    /// </summary>
    public BossStateSO ChooseNextAttackState()
    {
        // You can add more complex logic based on conditions.
        return (Random.Range(0, 2) == 0) ? clawState : groundSmashState;
    }

    /// <summary>
    /// Trigger the claw attack logic.
    /// </summary>
    public void PerformClawAttack()
    {
        Debug.Log("Performing Claw Attack!");
        // Additional in-state logic for claw attack can go here.
    }
}
