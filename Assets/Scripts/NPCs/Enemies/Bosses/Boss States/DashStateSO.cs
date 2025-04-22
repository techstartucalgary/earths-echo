using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Dash State")]
public class DashStateSO : BossStateSO
{
    public override void EnterState(TigerBossAttack boss)
    {
        //boss.PerformDashAttack();
        boss.animator.Play("tigerRunning");
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        // if (boss.DashFinished())
        // {
        //     boss.TransitionToState(boss.clawState);
        // }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exited Dash");
    }
}
