using UnityEngine;

public abstract class BossStateSO : ScriptableObject
{
    public abstract void EnterState(TigerBossAttack boss);
    public abstract void UpdateState(TigerBossAttack boss);
    public abstract void ExitState(TigerBossAttack boss);
}
