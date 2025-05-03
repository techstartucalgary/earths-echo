using UnityEngine;
using System.Collections;

public class EE_Plant : EE_NPC
{
    [Header("Combat")]
    [SerializeField] private float sideAttackInterval = 2f;

    private EnemyAttack enemyAttack;
    private Coroutine sideAttackCoroutine;
    private bool isSideAttacking = false;
    private bool isOnCooldown = false;

    [Header("Sensors")]
    [SerializeField] private Sensor upwardSensor;
    [SerializeField] private Sensor sideSensor;

    private float originalXScale;

    protected override void Start()
    {
        base.Start();
        enemyAttack = GetComponent<EnemyAttack>();

        if (target == null && GameObject.FindWithTag("Player") != null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        if (target != null)
        {
            upwardSensor.setTargetTag(target.tag);
            sideSensor.setTargetTag(target.tag);
        }

        if (npcGFX != null)
        {
            originalXScale = npcGFX.localScale.x;
        }
    }

    protected override void Update()
    {
        // Disable NPC pathing behavior
        if (enemyAttack == null || animator == null || target == null)
            return;

        // Flip to face the player
        FlipToFacePlayer();

        // Upward attack takes priority
        if (upwardSensor.IsTargetInRange)
        {
            animator.Play("plant_upward_repeated_attack");
            enemyAttack.triggerUpwardAttack = true;

            StopSideAttackIfActive();
            return;
        }

        // Side attack behavior
        if (sideSensor.IsTargetInRange && !isSideAttacking && !isOnCooldown)
        {
            sideAttackCoroutine = StartCoroutine(SideAttackRoutine());
            isSideAttacking = true;
        }
        else if (!sideSensor.IsTargetInRange && !upwardSensor.IsTargetInRange)
        {
            StopSideAttackIfActive();
            animator.Play("plant_idle");
        }
    }

    private void FlipToFacePlayer()
    {
        if (target.position.x < transform.position.x)
        {
            npcGFX.localScale = new Vector3(-Mathf.Abs(originalXScale), npcGFX.localScale.y, npcGFX.localScale.z);
        }
        else
        {
            npcGFX.localScale = new Vector3(Mathf.Abs(originalXScale), npcGFX.localScale.y, npcGFX.localScale.z);
        }
    }

    private IEnumerator SideAttackRoutine()
    {
        animator.Play("plant_attack1");
        enemyAttack.triggerSideAttack = true;

        // Wait one frame for transition to apply
        yield return null;

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        isSideAttacking = false;

        if (!sideSensor.IsTargetInRange && !upwardSensor.IsTargetInRange)
        {
            animator.Play("plant_idle");
        }
    }

    private void StopSideAttackIfActive()
    {
        if (sideAttackCoroutine != null)
        {
            StopCoroutine(sideAttackCoroutine);
            sideAttackCoroutine = null;
        }
        isSideAttacking = false;
        isOnCooldown = false;
    }
}
