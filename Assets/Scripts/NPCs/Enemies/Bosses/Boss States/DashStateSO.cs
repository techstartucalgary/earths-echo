using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Dash State")]
public class DashStateSO : BossStateSO
{
    public override void EnterState(TigerBossAttack boss)
    {
        //boss.PerformDashAttack();
<<<<<<< HEAD
        boss.animator.Play("tigerRunning");
=======
>>>>>>> 1f7daaef3e4cdd4bc1bcfbfa80104434ecbe6c5b
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
