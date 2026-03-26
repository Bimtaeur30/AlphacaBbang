using Unity.Behavior;

namespace JJH._02_Scripts.Agents.Enemies.BT
{
    [BlackboardEnum]
    public enum EnemyState
    {
        IDLE = 0,
        MOVE = 1,
        COMBAT = 2,
        HIT = 3,
        STUNNED = 4,
        DEAD = 5
    }
}
