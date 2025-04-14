using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Claw Attack State")]
public class ClawAttackStateSO : BossStateSO
{
    public float baseAttackRange = 1f;
    public float baseClawDamage = 10f;
    public float cooldownDuration = 2f;
    public float knockbackMultiplier = 1.5f;
    
    private bool hasAttacked;
    private float timer;

    public override void EnterState(TigerBossAttack boss)
    {
        Debug.Log("Entered Claw Attack State");
        hasAttacked = false;
        timer = 0f;
        // Check if the boss is in Phase2 and increase damage.
        float finalClawDamage = baseClawDamage;
        if (boss.GetComponent<BossFightManager>() != null && 
            ((BossFightManager)boss.GetComponent<BossFightManager>()).currentPhase == BossFightManager.BossPhase.Phase2)
        {
            finalClawDamage *= 1.5f; // Increase damage by 50% in phase two.
        }
        PerformClawAttack(boss, finalClawDamage);
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        timer += Time.deltaTime;
        if (timer >= cooldownDuration)
        {
            boss.TransitionToState(boss.idleState);
        }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exited Claw Attack State");
    }

    private void PerformClawAttack(TigerBossAttack boss, float clawDamage)
    {
        if (boss.clawHitPoint == null || hasAttacked)
            return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(boss.clawHitPoint.position, baseAttackRange, boss.TargetLayer);
        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector2 direction = (target.transform.position - boss.clawHitPoint.position).normalized;
                damageable.Damage(clawDamage, direction * boss.KnockbackForce * knockbackMultiplier);
            }
        }

        hasAttacked = true;
    }
}
