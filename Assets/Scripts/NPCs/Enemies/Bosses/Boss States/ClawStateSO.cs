using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Claw Attack State")]
public class ClawAttackStateSO : BossStateSO
{
    public float attackRange = 1f;
    public float clawDamage = 10f;
    public float cooldownDuration = 2f;
    public float knockbackMultiplier = 1.5f;
    
    private bool hasAttacked;
    private float timer;

    public override void EnterState(TigerBossAttack boss)
    {
        Debug.Log("Entered Claw Attack State");
        hasAttacked = false;
        timer = 0f;

        PerformClawAttack(boss);
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        timer += Time.deltaTime;

        if (timer >= cooldownDuration)
        {
            boss.TransitionToState(boss.idleState); // Go back to idle or next phase
        }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exited Claw Attack State");
    }

    private void PerformClawAttack(TigerBossAttack boss)
    {
        if (boss.clawHitPoint == null || hasAttacked)
            return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(boss.clawHitPoint.position, attackRange, boss.TargetLayer);

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
