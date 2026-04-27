using JJH._02_Scripts.Agents.Enemies.BT;
using JJH._02_Scripts.Agents.Enemies.BT.Channels;
using JJH._02_Scripts.Agents.Enemies.NavMeshs;
using JJH._02_Scripts.Systems.EventSystems;
using System.Collections;
using TMPro;
using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent, IDamageable
    {
        [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }
        [SerializeField] protected TextMeshPro _nameText;

        private BehaviorGraphAgent _btAgent;
        private BlackboardVariable<StateChannel> _stateChannel;

        private Coroutine _hitCoroutine;
        private Color _originColor;
        private Color _hitColor = new Color32(255, 50, 50, 255);
        private float _hitDuration = 0.3f;

        public INavMeshAgent NavMeshAgent { get; private set; }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            NavMeshAgent = GetModule<INavMeshAgent>();
            if (Weapon != null)
                Weapon.Init();
            HealthModule.InitHealth(EnemyData.EnemyHealth);
            _btAgent = GetComponent<BehaviorGraphAgent>();
            _originColor = Renderer.Renderer.material.color;
            _btAgent.BlackboardReference.GetVariable("StateChannel", out _stateChannel);
            _btAgent.SetVariableValue("Enemy", this);

            _nameText.text = EnemyData.EnemyName;
            AgentEventChannel.AddListener<AgentDeadEvent>(HandkeEnemyDeadEvent);
            AgentEventChannel.AddListener<AgentHealthChangeEvent>(HandkeEnemyHealthChangeEvent);
        }

        protected virtual void OnDestroy()
        {
            AgentEventChannel.RemoveListener<AgentDeadEvent>(HandkeEnemyDeadEvent);
            AgentEventChannel.RemoveListener<AgentHealthChangeEvent>(HandkeEnemyHealthChangeEvent);
        }

        private void HandkeEnemyDeadEvent(AgentDeadEvent evt)
        {
            _stateChannel.Value.SendEventMessage(EnemyState.DEAD);
        }

        private void HandkeEnemyHealthChangeEvent(AgentHealthChangeEvent evt)
        {
            if (_hitCoroutine != null)
                StopCoroutine(_hitCoroutine);

            _hitCoroutine = StartCoroutine(HitCoroutine());
        }

        private IEnumerator HitCoroutine()
        {
            float time = 0f;

            while (time < _hitDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Sin(time / _hitDuration * Mathf.PI);
                Renderer.Renderer.material.color = Color.Lerp(_originColor, _hitColor, t);
                yield return null;
            }

            Renderer.Renderer.material.color = _originColor;
        }

        public void Suicide()
        {

            _stateChannel.Value.SendEventMessage(EnemyState.DEAD);
            OnDead();
        }

        public void OnDead()
        {
            Debug.Log("적 사망");
            Instantiate(EnemyData.EnemyInventoryPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        public void ApplyBurn(float dps, float duration)
        {
            Debug.Log("적 불탐");
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"적 데미지 받음 : {damage}");
            HealthModule.SetHealth(damage);
        }
    }
}
