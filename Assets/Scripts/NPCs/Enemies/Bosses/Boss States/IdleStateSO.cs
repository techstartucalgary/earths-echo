using UnityEngine;

[CreateAssetMenu(menuName = "Boss States/Idle State")]
public class IdleStateSO : BossStateSO
{
    public float idleDuration = 2f;
    private float timer;

    public override void EnterState(TigerBossAttack boss)
    {
        timer = 0f;
        Debug.Log("Entered Idle");
    }

    public override void UpdateState(TigerBossAttack boss)
    {
        timer += Time.deltaTime;
        if (timer >= idleDuration)
        {
            boss.TransitionToState(boss.dashState); // Example transition
        }
    }

    public override void ExitState(TigerBossAttack boss)
    {
        Debug.Log("Exited Idle");
    }
}
