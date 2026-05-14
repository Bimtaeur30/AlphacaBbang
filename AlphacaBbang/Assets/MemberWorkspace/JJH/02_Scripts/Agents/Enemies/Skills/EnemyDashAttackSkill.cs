using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDashAttackSkill : MonoBehaviour, IEnemySkill
    {
        [Header("Dash")]
        [SerializeField] private float dashSpeed = 5f;
        [SerializeField] private float dashDistance = 12f;
        [SerializeField] private float playerCheckRadius = 0.5f;

        [Header("Damage")]
        [SerializeField] private float range = 3f;
        [SerializeField] private float maxDamage = 30f;
        [SerializeField] private float knockbackForce = 4f;
        [SerializeField] private LayerMask targetLayer;

        public bool IsDashing => _isDashing;

        private AbstractEnemy _owner;
        private NavMeshAgent _navMeshAgent;

        private HashSet<Collider> _hitTargets = new HashSet<Collider>();
        private Vector3 _dashStartPos;
        private Vector3 _dashDirection;
        private bool _isDashing;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
            _navMeshAgent = _owner.EnemyNavMeshAgent.NavMeshAgent;
        }

        private void FixedUpdate()
        {
            if (_isDashing == false)
                return;

            Dash();
        }

        public void UseSkill()
        {
            if (_isDashing)
                return;

            _hitTargets.Clear();
            _dashStartPos = transform.position;
            _dashDirection = transform.forward.normalized;
            _isDashing = true;

            _navMeshAgent.isStopped = true;
            _navMeshAgent.updateRotation = false;
        }

        private void Dash()
        {
            Vector3 move = _dashDirection * dashSpeed * Time.deltaTime;
            _navMeshAgent.Move(move);

            float moveDistance = move.magnitude;

            bool hitObstacle = Physics.SphereCast(transform.position + Vector3.up * 0.5f, playerCheckRadius,
                                                                        _dashDirection, out RaycastHit hit, moveDistance, targetLayer);

            if (hitObstacle)
            {
                EndDash();
                return;
            }

            _navMeshAgent.Move(move);

            DamageAround();

            float distance = Vector3.Distance(_dashStartPos, transform.position);
            if (distance >= dashDistance)
                EndDash();
        }

        private void EndDash()
        {
            _isDashing = false;

            _navMeshAgent.isStopped = false;
            _navMeshAgent.updateRotation = true;
        }

        private void DamageAround()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, range, targetLayer);
            foreach (Collider hit in hits)
            {
                if (hit.transform == transform)
                    continue;
                if (_hitTargets.Contains(hit))
                    continue;

                _hitTargets.Add(hit);

                Vector3 hitPoint = transform.position + Vector3.up;
                float distance = Vector3.Distance(hitPoint, hit.transform.position);

                float damage = Mathf.Lerp(maxDamage, 0f, distance / range);
                hit.GetComponent<IDamageable>()?.TakeDamage(damage);

                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null)
                {
                    Vector3 dir = (hit.transform.position - transform.position).normalized;
                    dir.y = 0.2f;
                    rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}