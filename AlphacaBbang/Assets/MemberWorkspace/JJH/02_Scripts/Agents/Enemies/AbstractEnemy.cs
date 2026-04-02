using Unity.Behavior;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent
    {
        public BehaviorGraphAgent BTAgent { get; private set; }
        public AttackDataSO AttackData { get; private set; }
    }
}
