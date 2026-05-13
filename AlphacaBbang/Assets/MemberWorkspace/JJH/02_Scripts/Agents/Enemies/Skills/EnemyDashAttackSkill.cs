using UnityEngine;
using UnityEngine.AI;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDashAttackSkill : MonoBehaviour, IEnemySkill
    {
        [Header("Dash")]
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashDuration = 0.4f;

        [Header("Damage")]
        [SerializeField] private float range = 2.5f;
        [SerializeField] private float maxDamage = 30f;
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private LayerMask targetLayer;

        private AbstractEnemy _owner;
        private NavMeshAgent _navMeshAgent;

        private bool _isDashing;
        private float _dashTime;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
            _navMeshAgent = owner.NavMeshAgent.NavMeshAgent;
        }

        public void UseSkill()
        {
            if (_isDashing)
                return;

            _isDashing = true;
            _dashTime = dashDuration;

            _navMeshAgent.isStopped = true;
            _navMeshAgent.updatePosition = false;
        }

        private void Update()
        {
            if (_isDashing == false)
                return;

            Dash();
        }

        private void Dash()
        {
            Vector3 move = transform.forward * dashSpeed * Time.deltaTime;

            transform.position += move;

            DamageAround();

            _dashTime -= Time.deltaTime;

            if (_dashTime <= 0f)
            {
                EndDash();
            }
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

        private void EndDash()
        {
            _isDashing = false;

            _navMeshAgent.Warp(transform.position);

            _navMeshAgent.updatePosition = true;
            _navMeshAgent.isStopped = false;
        }
    }
}