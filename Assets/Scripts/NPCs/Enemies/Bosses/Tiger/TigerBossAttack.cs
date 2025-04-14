using UnityEngine;

public class TigerBossAttack : EnemyAttack
{
    [Header("ScriptableObject States")]
    public BossStateSO idleState;
    public BossStateSO dashState;
    public BossStateSO clawState;
    public BossStateSO groundSmashState; // New ground smash state

    [Header("Rage Settings")]
    [Tooltip("Current rage level.")]
    public float currentRage = 0f;
    [Tooltip("Rage meter maximum value.")]
    public float maxRage = 100f;
    [Tooltip("Amount of rage increase per second or per event.")]
    public float rageIncreaseRate = 10f;

    [Header("Attack Triggers (Booleans)")]
    public bool triggerSideAttack;
    public bool triggerUpwardAttack;
    public bool triggerGroundSmashAttack; // Flag for ground smash trigger.
    public bool triggerClawAttack;

    [Header("Ground Smash Settings")]
    [Tooltip("The radius of the splash damage area for ground smash.")]
    public float splashDamageRadius = 5f;
    [Tooltip("Damage applied to each target in the splash area.")]
    public float groundSmashDamage = 20f;

    [Header("Dash Settings")]
    [Tooltip("Multiplier to the enemy speed during a dash.")]
    public float dashSpeedMultiplier = 2f; // Multiplies the tiger’s speed during dash

    // Reference to the Animator is needed by states (assign it in the inspector).
    public Animator animator;

    // Existing hit points and other properties...
    public Transform clawHitPoint;  // Used as the reference position for claw/hit detection and splash damage.
    public Transform downHitPoint;
    public LayerMask TargetLayer => targetLayer; // Assuming targetLayer is defined in a parent class.
    public float KnockbackForce => knockbackForce; // Assuming knockbackForce is defined in a parent class.

    private BossStateSO currentState;
    
    public EnemyAI enemyAI;

    protected void Start()
    {
        // Start the state machine in the idle state.
        TransitionToState(idleState);
    }

    protected override void Update()
    {
        base.Update();

        // Increase rage over time.
        IncreaseRage(rageIncreaseRate * Time.deltaTime);

        // If rage meter is full, trigger dash.
        if (currentRage >= maxRage)
        {
            currentRage = 0f; // Reset rage meter.
            TransitionToState(dashState); // Enter dash state.
            PerformDashAttack();
        }

        // Let the current state update its logic.
        currentState?.UpdateState(this);

        // After dashing, check if the dash is finished. You can refine this check as needed.
        if (currentState == dashState && DashFinished())
        {
            // After the dash, transition into a claw attack.
            TransitionToState(clawState);
            PerformClawAttack();
        }

        // Check for ground smash trigger (e.g., from a BossFightManager).
        if (triggerGroundSmashAttack)
        {
            triggerGroundSmashAttack = false;
            TransitionToState(groundSmashState);
            PerformGroundSmashAttack();
        }
    }

    // Transition from one state to another.
    public void TransitionToState(BossStateSO newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    // Increase the rage meter, clamping it between 0 and maxRage.
    public void IncreaseRage(float amount)
    {
        currentRage = Mathf.Clamp(currentRage + amount, 0, maxRage);
    }

    // Dash Attack: Increase speed and move aggressively toward the player.
    public void PerformDashAttack()
    {
        // Increase speed.
        enemyAI.speed *= dashSpeedMultiplier;

        // Determine the direction toward the player's position.
        Vector2 direction = ((Vector2)enemyAI.target.position - (Vector2)transform.position).normalized;

        // Set the velocity directly (assuming a Rigidbody2D is attached to enemyAI).
        Rigidbody2D rb = enemyAI.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * enemyAI.speed;
        }

        Debug.Log("Dashing with rage!");
    }

    // Check if the dash action is complete.
    public bool DashFinished()
    {
        // Here we use a simple check: if the enemy's velocity is very low.
        Rigidbody2D rb = enemyAI.GetComponent<Rigidbody2D>();
        if (rb != null && rb.velocity.magnitude < 0.1f)
        {
            return true;
        }
        return false;
    }

    // Claw Attack: Execute the claw hit logic.
    public void PerformClawAttack()
    {
        Debug.Log("Claw Attack!");
        // Insert your claw attack logic here.
        // For example, detect collisions at clawHitPoint and apply damage.
    }

    // Ground Smash Attack: Execute ground smash logic and apply splash damage.
    public void PerformGroundSmashAttack()
    {
        Debug.Log("Ground Smash Attack!");

        // Optionally, play a ground smash animation or visual effect here.

        // Use Physics2D.OverlapCircleAll to get all colliders in the splash damage area.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(downHitPoint.position, splashDamageRadius, TargetLayer);
        foreach (Collider2D hit in hitColliders)
        {
            // If your targets have a Damageable component, call its TakeDamage method.
            IDamageable damageable = hit.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(groundSmashDamage);
            }
        }
    }
}
