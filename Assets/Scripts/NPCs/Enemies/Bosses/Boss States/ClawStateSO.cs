using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Claw Attack State")]
public class ClawStateSO : BossStateSO
{
    [Tooltip("Force applied for the dash toward the player.")]
    public float dashForce = 500f;
    [Tooltip("Base damage of the claw attack.")]
    public float baseClawDamage = 10f;
    [Tooltip("Cooldown duration before transitioning after the claw attack.")]
    public float cooldownDuration = 1.0f;
    [Tooltip("Knockback multiplier applied to targets on attack.")]
    public float knockbackMultiplier = 1.5f;

    private float timer;
    private bool dashInitiated;

    public override void EnterState(TigerBossAttack boss)
    {
        boss.animator.SetTrigger("ClawAttack");
<<<<<<< HEAD
        boss.animator.Play("tigerAttacking");
=======
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
        Debug.Log("Entered Claw Attack State (Dash via force).");

        timer = 0f;
        dashInitiated = false;

        Rigidbody2D rb = boss.GetComponent<Rigidbody2D>();
        if (rb != null && boss.enemyAI != null && boss.enemyAI.target != null)
        {
            // Compute the normalized direction vector from the boss to the player.
            Vector2 direction = ((Vector2)boss.enemyAI.target.position - (Vector2)boss.transform.position).normalized;
            // Reset current velocity to ensure a clean dash.
            rb.velocity = Vector2.zero;
            // Apply an impulse force for the dash.
            rb.AddForce(direction * dashForce);
            dashInitiated = true;
        }

        // Immediately perform the claw attack for damage.
        float finalClawDamage = baseClawDamage;
        if (boss.isPhaseTwo)
            finalClawDamage *= 1.5f;
        PerformClawAttack(boss, finalClawDamage);
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        timer += Time.deltaTime;
        if (timer >= cooldownDuration)
        {
            // After the claw attack cooldown, return to rage state if rage is still active;
            // otherwise, transition to idle.
            if (boss.isRaging)
                boss.TransitionToState(boss.rageState);
            else
                boss.TransitionToState(boss.idleState);
        }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exited Claw Attack State.");
    }

    private void PerformClawAttack(TigerBossAttack boss, float clawDamage)
    {
        if (boss.clawHitPoint == null)
            return;
            
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(boss.clawHitPoint.position, 1f, boss.TargetLayer);
        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector2 direction = (target.transform.position - boss.clawHitPoint.position).normalized;
                damageable.Damage(clawDamage, direction * boss.KnockbackForce * knockbackMultiplier);
<<<<<<< HEAD
                SoundFXManager.Instance.PlaySoundFXClip(boss.clawSound, boss.transform, 1f);
=======
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
            }
        }
    }
}
