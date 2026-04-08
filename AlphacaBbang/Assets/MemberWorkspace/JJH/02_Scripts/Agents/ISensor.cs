using UnityEngine;

namespace JJH._02_Scripts.Agents
{
    public interface ISensor
    {
        public bool IsTargetInRange(float range, out Collider hitCollider);
        public bool IsTargetInSight(Vector3 startPosition, float range, Collider target);
    }
}