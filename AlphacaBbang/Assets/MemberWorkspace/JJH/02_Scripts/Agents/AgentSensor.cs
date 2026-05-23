using JJH._02_Scripts.Agents.Enemies;
using System.Linq;
using UnityEngine;

namespace JJH._02_Scripts.Agents
{
    public class AgentSensor : MonoBehaviour, IModule, ISensor
    {
        [Header("Layer")]
        [field: SerializeField] public LayerMask ObstacleLayer { get; private set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }
        [field: SerializeField] public LayerMask SmokeLayer { get; private set; }

        [Header("Layer")]
        [SerializeField] private float selfCheckRadius;
        [SerializeField] private float viewAngle = 120f;

        private Agent _owner;

        private float _debugRange = 0;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
        }

        public bool IsTargetInRange(float range, out Collider hitCollider)
        {
            hitCollider = Physics.OverlapSphere(transform.position, range, TargetLayer).FirstOrDefault();
            _debugRange = range;
            return hitCollider != null;
        }

        public bool IsTargetInSight(Vector3 startPosition, Transform target = null)
        {
            Vector3 direction;
            float distance = _owner is AbstractEnemy ? ((AbstractEnemy)_owner).EnemyData.DetectRange : 100f;

            if (target != null)
            {
                direction = (target.position - startPosition).normalized;
                distance = Vector3.Distance(startPosition, target.position);
            }
            else
            {
                direction = _owner.transform.forward;
            }

            float angle = Vector3.Angle(_owner.transform.forward, direction);
            if (angle > viewAngle * 0.5f)
                return false;

            if (target != null)
                return !Physics.Raycast(startPosition, direction, distance, ObstacleLayer);
            else
            {
                Collider hit = Physics.OverlapSphere(startPosition, distance, TargetLayer).FirstOrDefault();
                if (hit == null) return
                        false;

                Vector3 toTarget = (hit.transform.position - startPosition).normalized;
                if (Vector3.Angle(_owner.transform.forward, toTarget) > viewAngle * 0.5f)
                    return false;

                return !Physics.Raycast(startPosition, toTarget, Vector3.Distance(startPosition, hit.transform.position), ObstacleLayer);
            }
        }

        public bool CheckAgentInSmoke()
        {
            Collider hitCollider = Physics.OverlapSphere(transform.position, selfCheckRadius, SmokeLayer).FirstOrDefault();
            _debugRange = selfCheckRadius;
            return hitCollider != null;
        }

        private void OnDrawGizmos()
        {
            _debugRange = selfCheckRadius;
            if (_debugRange > 0f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, _debugRange);
            }
        }
    }
}