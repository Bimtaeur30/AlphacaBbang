using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : MonoBehaviour
    {
        public BehaviorGraphAgent BTAgent { get; private set; }
        public AgentSensor Sensor { get; private set; }

        public AttackDataSO AttackData { get; private set; }
    }
}
