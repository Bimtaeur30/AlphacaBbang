using System.Linq;
using UnityEngine;

namespace JJH._02_Scripts.Agents
{
    public class AgentSensor : MonoBehaviour, IModule, ISensor
    {
        [field: SerializeField] public LayerMask ObstacleLayer { get; private set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }

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

        public bool IsTargetInSight(Vector3 startPosition, Transform target)
        {
            Vector3 direction = target.transform.position - startPosition;

            RaycastHit hit;
            bool isHit = Physics.Raycast(startPosition, direction.normalized,
                                                         out hit, direction.magnitude, ObstacleLayer);

            Debug.DrawLine(startPosition, target.transform.position, Color.red, 0.1f);

            return !isHit;
        }

        private void OnDrawGizmos()
        {
            if (_debugRange > 0f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, _debugRange);
            }
        }
    }
}