using UnityEngine;
using System.Collections;

public class EE_Snake : EE_NPC
{
    [Header("Combat")]
    [SerializeField] private float attackTriggerDistance = 1.5f;
    [SerializeField] private float attackInterval = 1.5f; // Time between attack attempts

    private EnemyAttack enemyAttack;
    private Coroutine   attackRoutine;
    private bool isAttacking = false;

    [SerializeField] private Sensor actionSensor;
    float actionSensorRadius;
    


    protected override void Start()
    {
        base.Start();                                              // sets up movement

        enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack == null)
        {
            Debug.LogError("[EE_Snake] Missing <EnemyAttack> component – disabled.");
            enabled = false;
            return;
        }

        if (actionSensor == null) actionSensor = GetComponent<Sensor>();
        if (actionSensor == null)
        {
            Debug.LogError("[EE_Snake] Missing action Sensor – disabled.");
            enabled = false;
        }
    }


    protected override void Update()
    {
        base.Update();                                             // handles locomotion

        if (target == null || actionSensor == null) return;

        bool targetInRange = actionSensor.IsTargetInRange;

        if (targetInRange && !isAttacking)
        {
            attackRoutine = StartCoroutine(AttackLoop());
            isAttacking   = true;
        }
        else if (!targetInRange && isAttacking)
        {
            StopCoroutine(attackRoutine);
            isAttacking = false;
        }

        // Face the target when attacking
        if (isAttacking)
        {
            if (target.position.x < transform.position.x)
                 npcGFX.localScale = new Vector3(-animatorXScale, npcGFX.localScale.y, npcGFX.localScale.z);
            else npcGFX.localScale = new Vector3( animatorXScale, npcGFX.localScale.y, npcGFX.localScale.z);
        }
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            if (enemyAttack.Attack(enemyAttack.SideHitPoint))
                animator?.Play("snake_attack");

            yield return new WaitForSeconds(attackInterval);
        }
    }


}
