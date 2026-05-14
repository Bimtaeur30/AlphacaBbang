using JJH._02_Scripts.Agents.Enemies.BT;
using JJH._02_Scripts.Agents.Enemies.BT.Channels;
using JJH._02_Scripts.Agents.Enemies.NavMeshs;
using JJH._02_Scripts.Agents.Enemies.Skills;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Weapons;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent, IDamageable
    {
        [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }
        [SerializeField] private WeaponBase[] Weapons;

        public IEnemySkillModule EnemySkill { get; private set; }
        public IEnemyInterface EnemyInterface { get; private set; }
        public INavMeshAgent EnemyNavMeshAgent { get; private set; }

        private BehaviorGraphAgent _btAgent;
        private BlackboardVariable<StateChannel> _stateChannel;

        private Coroutine _hitCoroutine;
        private Color _originColor;
        private Color _hitColor = new Color32(255, 255, 255, 255);
        private float _hitDuration = 0.15f;

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            int rand = Random.Range(0, Weapons.Length);
            if (Weapon is EnemyGunHandleModule)
            {
                Weapons[rand].gameObject.SetActive(true);
                EnemyGunHandleModule gunHandleModule = (EnemyGunHandleModule)Weapon;
                gunHandleModule.SetCurrentGun((Gun)Weapons[rand]);
            }
            else if (Weapon is AgentAttack)
            {
                Weapons[rand].gameObject.SetActive(true);
                AgentAttack gunHandleModule = (AgentAttack)Weapon;
                gunHandleModule.SetCurrentWeapon((MeleeWeaponBase)Weapons[rand]);
            }
            if (Weapon != null)
                Weapon.Init();

            EnemyNavMeshAgent = GetModule<INavMeshAgent>();
            EnemySkill = GetModule<IEnemySkillModule>();
            EnemyInterface = GetModule<IEnemyInterface>();

            HealthModule.InitHealth(EnemyData.EnemyHealth);

            _btAgent = GetComponent<BehaviorGraphAgent>();
            _originColor = Renderer.Renderer.material.color;
            _btAgent.BlackboardReference.GetVariable("StateChannel", out _stateChannel);
            _btAgent.SetVariableValue("Enemy", this);

            AgentEventChannel.AddListener<AgentDeadEvent>(HandkeEnemyDeadEvent);
            AgentEventChannel.AddListener<AgentHealthChangeEvent>(HandkeEnemyHealthChangeEvent);
            AgentEventChannel.AddListener<AgentInventoryDropEvent>(HandkeEnemyInventoryDropEvent);
        }

        protected virtual void OnDestroy()
        {
            AgentEventChannel.RemoveListener<AgentDeadEvent>(HandkeEnemyDeadEvent);
            AgentEventChannel.RemoveListener<AgentHealthChangeEvent>(HandkeEnemyHealthChangeEvent);
            AgentEventChannel.RemoveListener<AgentInventoryDropEvent>(HandkeEnemyInventoryDropEvent);
        }

        private void HandkeEnemyDeadEvent(AgentDeadEvent evt)
        {
            if (evt.Agent == this)
            {
                _stateChannel.Value.SendEventMessage(EnemyState.DEAD);
                EnemyInterface.SetInterfaceShow(false);
            }
        }

        private void HandkeEnemyHealthChangeEvent(AgentHealthChangeEvent evt)
        {
            if (_hitCoroutine != null)
                StopCoroutine(_hitCoroutine);

            _hitCoroutine = StartCoroutine(HitCoroutine());
        }

        private void HandkeEnemyInventoryDropEvent(AgentInventoryDropEvent evt)
        {
            if (evt.Agent == this)
            {
                OnDead();
            }
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
            EnemySkill.UseSkill<EnemyBombSkill>();
            _stateChannel.Value.SendEventMessage(EnemyState.DEAD);
            OnDead();
        }

        public void DashAttack()
        {
            EnemySkill.UseSkill<EnemyDashAttackSkill>();
        }

        public void DashAttackStandBy()
        {
            EnemySkill.UseSkill<EnemyDashAttackStandBySkill>();
        }

        public void OnDead()
        {
            Instantiate(EnemyData.EnemyInventoryPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        public void ApplyBurn(float dps, float duration)
        {
        }

        public void TakeDamage(float damage)
        {
            HealthModule.SetHealth(damage);
        }
    }
}
