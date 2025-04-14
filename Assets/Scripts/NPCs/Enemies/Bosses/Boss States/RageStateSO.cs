using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Rage State")]
public class RageStateSO : BossStateSO
{
    [Tooltip("Amplitude of lateral sin wave movement.")]
    public float amplitude = 2f;
    [Tooltip("Frequency of sin wave oscillation.")]
    public float frequency = 2f;
    [Tooltip("Force multiplier to apply for horizontal movement.")]
    public float forceMultiplier = 10f;
    [Tooltip("Maximum horizontal speed allowed.")]
    public float maxHorizontalSpeed = 5f;
    [Tooltip("Threshold distance (on x-axis) to trigger the claw attack.")]
    public float contactThreshold = 0.5f;

    public override void EnterState(TigerBossAttack boss)
    {
        boss.animator.SetTrigger("Rage");
        Debug.Log("Entered Rage State (Force-based movement).");
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        Rigidbody2D rb = boss.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("RageStateSO: No Rigidbody2D found on boss.");
            return;
        }
        
        // Calculate the desired target x-position based on a sine wave.
        float targetX = boss.initialX + amplitude * Mathf.Sin(Time.time * frequency);
        float diff = targetX - boss.transform.position.x;
        
        // Compute a horizontal force proportional to the distance from the target position.
        float force = diff * forceMultiplier;
        rb.AddForce(new Vector2(force, 0));
        
        // Clamp the horizontal velocity.
        Vector2 vel = rb.velocity;
        if (Mathf.Abs(vel.x) > maxHorizontalSpeed)
        {
            vel.x = Mathf.Sign(vel.x) * maxHorizontalSpeed;
            rb.velocity = vel;
        }
        
        // Check whether the boss is close enough to the player to trigger a claw attack.
        if (boss.enemyAI != null && boss.enemyAI.target != null)
        {
            float distanceToTarget = Mathf.Abs(boss.transform.position.x - boss.enemyAI.target.position.x);
            if (distanceToTarget < contactThreshold)
            {
                // Stop horizontal motion before transitioning.
                rb.velocity = new Vector2(0, rb.velocity.y);
                boss.TransitionToState(boss.clawState);
            }
        }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exiting Rage State (Force-based movement).");
    }
}
