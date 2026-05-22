using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.ParticleSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDashAttackSkill : MonoBehaviour, IEnemySkill
    {
        [Header("Dash")]
        [SerializeField] private float dashAcceleration = 20f;
        [SerializeField] private float maxDashSpeed = 12f;
        [SerializeField] private float dashDistance = 12f;
        [SerializeField] private float playerCheckRadius = 0.5f;

        [Header("Damage")]
        [SerializeField] private float range = 3f;
        [SerializeField] private float maxDamage = 30f;
        [SerializeField] private float knockbackForce = 4f;
        [SerializeField] private LayerMask targetLayer;

        [Header("Particle")]
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO dashParticlePref;
        [SerializeField] private Transform particleSpawnPos;
        [SerializeField] private float particleSpawnInterval = 0.1f;

        public bool IsDashing => _isDashing;

        private AbstractEnemy _owner;
        private NavMeshAgent _navMeshAgent;

        private readonly HashSet<Collider> _hitTargets = new();

        private Vector3 _dashStartPos;
        private Vector3 _dashDirection;

        private bool _isDashing;
        private float _particleTimer;
        private float _currentDashSpeed;

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
            _particleTimer = 0f;
            _currentDashSpeed = maxDashSpeed / 2;

            _navMeshAgent.ResetPath();
            _navMeshAgent.isStopped = true;
            _navMeshAgent.updateRotation = false;
        }

        private void Dash()
        {
            _particleTimer += Time.fixedDeltaTime;
            if (_particleTimer >= particleSpawnInterval)
            {
                _particleTimer = 0f;

                DashAttackParticle effect = poolManager.Pop<DashAttackParticle>(dashParticlePref);
                effect.transform.position = particleSpawnPos.position;
                effect.transform.rotation = Quaternion.LookRotation(-_dashDirection);
                effect.PlayDashParticle();
            }

            _currentDashSpeed += dashAcceleration * Time.fixedDeltaTime;
            _currentDashSpeed = Mathf.Min(_currentDashSpeed, maxDashSpeed);
            Vector3 move = _dashDirection * _currentDashSpeed * Time.fixedDeltaTime;

            bool isHit = Physics.SphereCast(transform.position + Vector3.up * 0.5f, playerCheckRadius,
                                                              _dashDirection, out RaycastHit hit, move.magnitude, targetLayer);
            if (isHit)
            {
                EndDash();
                return;
            }

            _navMeshAgent.Move(move);

            DamageAround();

            float distance = Vector3.Distance(_dashStartPos, transform.position);

            if (distance >= dashDistance)
            {
                EndDash();
            }
        }

        private void EndDash()
        {
            _isDashing = false;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.updateRotation = true;
        }

        private void DamageAround()
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                range,
                targetLayer);

            foreach (Collider hit in hits)
            {
                if (hit.transform == transform)
                    continue;

                if (_hitTargets.Contains(hit))
                    continue;

                _hitTargets.Add(hit);

                Vector3 hitPoint = transform.position + Vector3.up;

                float distance = Vector3.Distance(
                    hitPoint,
                    hit.transform.position);

                float damage = Mathf.Lerp(
                    maxDamage,
                    0f,
                    distance / range);

                hit.GetComponent<IDamageable>()?.TakeDamage(damage);

                Rigidbody rb = hit.attachedRigidbody;

                if (rb != null)
                {
                    Vector3 dir =
                        (hit.transform.position - transform.position).normalized;

                    dir.y = 0.2f;

                    rb.AddForce(
                        dir * knockbackForce,
                        ForceMode.Impulse);
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