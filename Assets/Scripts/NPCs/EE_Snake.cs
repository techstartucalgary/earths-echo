using UnityEngine;
using System.Collections;

public class EE_Snake : EE_NPC
{
    [Header("Combat")]
    [SerializeField] private float attackTriggerDistance = 1.5f;
    [SerializeField] private float attackInterval = 1.5f; // Time between attack attempts

    private EnemyAttack enemyAttack;
    private Coroutine attackCoroutine;
    private bool isAttacking = false;

    [SerializeField] private Sensor actionSensor;
    float actionSensorRadius;


    protected override void Start()
    {
        base.Start();
        enemyAttack = GetComponent<EnemyAttack>();
        actionSensor.setTargetTag(base.target.tag);
		actionSensorRadius = actionSensor.detectionRadius;
    }

    protected override void Update()
    {

        base.Update();

        if (enemyAttack == null)
        {
            Debug.LogError("EE_Snake requires an EnemyAttack component.");
        }

        if (target == null) return;


        if (actionSensor.IsTargetInRange)
        {
            // Start attack coroutine if not already started
            if (!isAttacking)
            {
                Debug.Log("Attack coroutine started");
                attackCoroutine = StartCoroutine(AttackRoutine());
                isAttacking = true;
            }

            // Face the target
            if (target.position.x < transform.position.x)
                npcGFX.localScale = new Vector3(-animatorXScale, npcGFX.localScale.y, npcGFX.localScale.z);
            else
                npcGFX.localScale = new Vector3(animatorXScale, npcGFX.localScale.y, npcGFX.localScale.z);
        }
        else
        {
            // Stop attack coroutine when out of range
            if (isAttacking)
            {
                StopCoroutine(attackCoroutine);
                isAttacking = false;
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (enemyAttack != null && enemyAttack.Attack(enemyAttack.SideHitPoint))
            {
                animator.Play("snake_attack"); // if you have a specific animation
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }


}
