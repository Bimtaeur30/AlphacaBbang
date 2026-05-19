using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.ParticleSystems;
using UnityEngine;
using UnityEngine.AI;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDashSkill : MonoBehaviour, IEnemySkill
    {
        [Header("Dash")]
        [SerializeField] private float dashAcceleration = 20f;
        [SerializeField] private float maxDashSpeed = 12f;
        [SerializeField] private float dashDistance = 12f;

        [Header("Particle")]
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO dashParticlePref;
        [SerializeField] private Transform particleSpawnPos;
        [SerializeField] private float particleSpawnInterval = 0.1f;

        public bool IsDashing => _isDashing;

        private AbstractEnemy _owner;
        private NavMeshAgent _navMeshAgent;

        private bool _isDashing;
        private float _particleTimer;
        private float _currentDashSpeed;
        private float _movedDistance;

        private Vector3 _dashDirection;

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

            _isDashing = true;

            _particleTimer = 0f;
            _movedDistance = 0f;
            _currentDashSpeed = maxDashSpeed / 2;

            _dashDirection = transform.forward;
            _dashDirection.y = 0f;
            _dashDirection.Normalize();

            _navMeshAgent.ResetPath();
            _navMeshAgent.isStopped = true;
            _navMeshAgent.updateRotation = false;
        }

        private void Dash()
        {
            SpawnParticle();

            _currentDashSpeed += dashAcceleration * Time.fixedDeltaTime;
            _currentDashSpeed = Mathf.Min(_currentDashSpeed, maxDashSpeed);

            float moveAmount = _currentDashSpeed * Time.fixedDeltaTime;
            if (_movedDistance + moveAmount >= dashDistance)
            {
                moveAmount = dashDistance - _movedDistance;
            }

            Vector3 move = _dashDirection * moveAmount;
            _navMeshAgent.Move(move);
            _movedDistance += moveAmount;

            if (_movedDistance >= dashDistance)
            {
                EndDash();
            }
        }

        private void SpawnParticle()
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
        }

        private void EndDash()
        {
            _isDashing = false;

            _navMeshAgent.isStopped = false;
            _navMeshAgent.updateRotation = true;
        }
    }
}