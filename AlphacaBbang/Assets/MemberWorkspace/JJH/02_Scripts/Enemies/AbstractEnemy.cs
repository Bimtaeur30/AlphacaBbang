using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Enemies
{
    public abstract class AbstractEnemy : MonoBehaviour
    {
        public BehaviorGraphAgent BTAgent { get; private set; }

    }
}
