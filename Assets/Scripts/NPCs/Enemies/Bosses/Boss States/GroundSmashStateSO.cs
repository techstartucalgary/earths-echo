using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Boss States/Ground Smash State")]
public class GroundSmashStateSO : BossStateSO
{
    [Header("Ground Smash Settings")]
    [Tooltip("Force applied upward for the jump.")]
    public float jumpForce = 10f;
    [Tooltip("Delay (in seconds) before performing the smash after the jump.")]
    public float landingDelay = 0.5f;
    [Tooltip("Radius of the splash damage.")]
    public float smashRadius = 2f;
    [Tooltip("Damage dealt by the smash.")]
    public float smashDamage = 20f;
    [Tooltip("Layers to damage.")]
    public LayerMask damageLayers;

    private bool hasSmashed;

    public override void EnterState(TigerBossAttack boss)
    {
        hasSmashed = false;
        // Play the ground smash animation.
        boss.animator.SetTrigger("GroundSmash");

        // Make the boss jump.
        Rigidbody2D rb = boss.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        if (!hasSmashed)
        {
            // Use a coroutine to wait for landing and then perform smash.
            boss.StartCoroutine(PerformSmash(boss));
            hasSmashed = true;
        }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exited Ground Smash State");
    }

    private IEnumerator PerformSmash(TigerBossAttack boss)
    {
        // Wait a small delay simulating "landing".
        yield return new WaitForSeconds(landingDelay);

        // Determine the position for the smash (boss's position).
        Vector2 smashPosition = boss.transform.position;

        // Find all targets within smash radius.
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(smashPosition, smashRadius, damageLayers);
        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Apply damage. Damage direction is set to zero (or can be calculated as needed).
                damageable.Damage(smashDamage, Vector2.zero);
            }
        }

        // Optionally, play a sound effect or spawn a particle system for impact.

        // Transition back to the idle state when done.
        boss.TransitionToState(boss.idleState);
    }
}
