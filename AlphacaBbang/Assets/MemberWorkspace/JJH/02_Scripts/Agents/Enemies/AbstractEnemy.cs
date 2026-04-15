using JJH._02_Scripts.Systems.EventSystems;
using MemberWorkspace.JJH._02_Scripts.Agents.Enemies.NavMesh;
using TMPro;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent, IDamageable
    {
        [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }
        [field: SerializeField] public TextMeshPro nameText;

        public INavMeshAgent NavMeshAgent { get; private set; }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            NavMeshAgent = GetModule<INavMeshAgent>();
            Weapon.Init();
            HealthModule.InitHealth(EnemyData.EnemyHealth);

            nameText.text = EnemyData.EnemyName;

            AgentEventChannel.AddListener<AgentDeadEvent>(HandkeAgentDeadEvent);
        }

        protected virtual void OnDestroy()
        {
            AgentEventChannel.RemoveListener<AgentDeadEvent>(HandkeAgentDeadEvent);
        }

        private void HandkeAgentDeadEvent(AgentDeadEvent evt)
        {
            Debug.Log("적 사망");
            Destroy(gameObject);
        }

        public void ApplyBurn(float dps, float duration)
        {
            Debug.Log("적 불탐");
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("적 데미지 받음");
            HealthModule.SetHealth(damage);
        }
    }
}
