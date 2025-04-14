using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Boss States/Ground Smash State")]
public class GroundSmashStateSO : BossStateSO
{
    [Header("Ground Smash Settings")]
    [Tooltip("Upward force applied for the jump.")]
    public float jumpForce = 10f;
    [Tooltip("Delay before performing the smash after the jump (seconds).")]
    public float landingDelay = 0.5f;
    [Tooltip("Radius within which targets will be affected by splash damage.")]
    public float smashRadius = 2f;
    [Tooltip("Damage applied by the smash.")]
    public float smashDamage = 20f;
    [Tooltip("Layers that can be damaged.")]
    public LayerMask damageLayers;

    private bool hasSmashed;

    public override void EnterState(TigerBossAttack boss)
    {
        hasSmashed = false;
        boss.animator.SetTrigger("GroundSmash");
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
        yield return new WaitForSeconds(landingDelay);
        Vector2 smashPosition = boss.transform.position;
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(smashPosition, smashRadius, damageLayers);
        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(smashDamage, Vector2.zero);
            }
        }
        boss.TransitionToState(boss.idleState);
    }
}
