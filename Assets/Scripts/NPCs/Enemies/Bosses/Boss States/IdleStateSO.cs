using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Idle State")]
public class IdleStateSO : BossStateSO
{
    [Tooltip("How long the boss stays idle before choosing the next attack.")]
    public float idleDuration = 2f;
    private float timer;

    public override void EnterState(TigerBossAttack boss)
    {
        timer = 0f;
        Debug.Log("Entered Idle State");
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        timer += Time.deltaTime;
        if (timer >= idleDuration)
        {
            // Centralize next attack decision in TigerBossAttack.
            boss.TransitionToState(boss.ChooseNextAttackState());
        }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exited Idle State");
    }
}
