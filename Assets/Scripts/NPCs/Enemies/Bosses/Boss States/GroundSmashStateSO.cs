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
    
    [Header("Ground‑Smash VFX")]
	public GameObject jumpParticlePrefab;     // spawn when the boss takes off
	public GameObject smashParticlePrefab;    // spawn when the boss lands


    private bool hasSmashed;

	public override void EnterState(TigerBossAttack boss)
	{
		hasSmashed = false;
		boss.animator.SetTrigger("GroundSmash");
		boss.animator.Play("tigerJump");

		// 🔸 Jump‑blast
		if (jumpParticlePrefab != null)
			Object.Instantiate(jumpParticlePrefab, boss.transform.position, Quaternion.identity);

		var rb = boss.GetComponent<Rigidbody2D>();
		if (rb != null)
		{
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			SoundFXManager.Instance.PlaySoundFXClip(boss.groundPoundSound, boss.transform, 1f);
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
        if (smashParticlePrefab != null)
			Object.Instantiate(smashParticlePrefab, boss.transform.position, Quaternion.identity);
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(smashPosition, smashRadius, damageLayers);
        foreach (Collider2D target in hitTargets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(smashDamage, Vector2.zero);
                SoundFXManager.Instance.PlaySoundFXClip(boss.clawSound, boss.transform, 1f);

            }
        }
        boss.TransitionToState(boss.idleState);
    }
}
