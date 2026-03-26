using System.Linq;
using UnityEngine;

namespace JJH._02_Scripts.Agents
{
    public class AgentSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask targetLayer;

        [SerializeField] private Vector3 boxSize;
        [SerializeField] private Vector3 offset;

        public bool IsTargetInRange(float range, out Collider hitCollider)
        {
            hitCollider = Physics.OverlapSphere(transform.position, range, targetLayer).FirstOrDefault();

            return hitCollider != null;
        }

        public bool IsTargetInSight(Vector3 startPosition, float range, Collider target)
        {
            Vector3 direction = target.transform.position - startPosition;

            RaycastHit hit;
            bool isHit = Physics.Raycast(startPosition, direction.normalized,
                out hit, direction.magnitude, obstacleLayer);

            return !isHit;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + offset, boxSize);
        }
    }
}