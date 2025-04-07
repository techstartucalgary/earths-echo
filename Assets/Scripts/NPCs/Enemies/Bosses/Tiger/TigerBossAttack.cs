using UnityEngine;

public class TigerBossAttack : EnemyAttack
{
    [Header("ScriptableObject States")]
    public BossStateSO idleState;
    public BossStateSO dashState;
    public BossStateSO clawState;

    private BossStateSO currentState;

    public Transform clawHitPoint;

    public LayerMask TargetLayer => targetLayer;
    public float KnockbackForce => knockbackForce;

    protected  void Start()
    {
        TransitionToState(idleState);
    }

    protected override void Update()
    {
        base.Update();
        currentState?.UpdateState(this);
    }

    public void TransitionToState(BossStateSO newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    public void PerformDashAttack()
    {
        // Your dash logic
        Debug.Log("Dashing!");
    }

    public bool DashFinished()
    {
        // Your logic to determine if dash is done
        return true;
    }

    public void PerformClawAttack()
    {
        // Your claw logic
        Debug.Log("Claw Attack!");
    }
}
