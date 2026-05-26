using JJH._02_Scripts.Agents.Enemies.BT;
using JJH._02_Scripts.Agents.Enemies.BT.Channels;
using JJH._02_Scripts.Agents.Enemies.NavMeshs;
using JJH._02_Scripts.Agents.Enemies.Skills;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Weapons;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JJH._02_Scripts.Agents.Enemies
{
    public abstract class AbstractEnemy : Agent
    {
        [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }
        [field: SerializeField] public LootTable[] LootTables { get; private set; }
        [SerializeField] private EventChannelSO mapChnnel;

        public IEnemySkillModule EnemySkill { get; private set; }
        public IEnemyInterface EnemyInterface { get; private set; }
        public INavMeshAgent EnemyNavMeshAgent { get; private set; }

        public BlackboardVariable<StateChannel> StateChannel => _stateChannel;
        private BlackboardVariable<StateChannel> _stateChannel;

        public int WeaponNum => _weaponNum;
        private int _weaponNum = 0;

        private BehaviorGraphAgent _btAgent;

        private Coroutine _hitCoroutine;
        private Color[] _originColors;
        private Color _hitColor = new Color32(255, 255, 255, 255);
        private float _hitDuration = 0.15f;

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            if (EnemyData.Weapons != null && EnemyData.Weapons.Length != 0)
                _weaponNum = Random.Range(0, EnemyData.Weapons.Length);
            if (Weapon is EnemyWeaponHandleModule)
            {
                EnemyData.Weapons[_weaponNum].gameObject.SetActive(true);
                EnemyWeaponHandleModule gunHandleModule = (EnemyWeaponHandleModule)Weapon;
                gunHandleModule.SetCurrentGun((Gun)EnemyData.Weapons[_weaponNum]);
            }
            else if (Weapon is AgentAttack)
            {
                EnemyData.Weapons[_weaponNum].gameObject.SetActive(true);
                AgentAttack gunHandleModule = (AgentAttack)Weapon;
                gunHandleModule.SetCurrentWeapon((MeleeWeaponBase)EnemyData.Weapons[_weaponNum]);
            }
            if (Weapon != null)
                Weapon.Init();

            if (EnemyData.HelmetArmor != null)
                Armor.ArmorEquip(true, ArmorType.Helmet, EnemyData.HelmetArmor);
            if (EnemyData.ShieldArmor != null)
                Armor.ArmorEquip(true, ArmorType.Body, EnemyData.ShieldArmor);

            EnemyNavMeshAgent = GetModule<INavMeshAgent>();
            EnemySkill = GetModule<IEnemySkillModule>();
            EnemyInterface = GetModule<IEnemyInterface>();

            HealthModule.InitHealth(EnemyData.EnemyHealth);

            _originColors = new Color[Renderer.Materials.Length];
            for (int i = 0; i < Renderer.Materials.Length; i++)
                _originColors[i] = Renderer.Materials[i].color;

            _btAgent = GetComponent<BehaviorGraphAgent>();
            if (_btAgent != null)
            {
                _btAgent.BlackboardReference.GetVariable("StateChannel", out _stateChannel);
                _btAgent.SetVariableValue("Enemy", this);
            }

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
            if (evt.Agent == this)
            {
                _stateChannel.Value.SendEventMessage(EnemyState.DEAD);
                mapChnnel.RaiseEvent(MapEvents.PlayerActionEvent.Init($"플레이어가 {EnemyData.EnemyName}을/를 처치했습니다."));
                EnemyInterface.SetInterfaceShow(false);
            }
        }

        private void HandkeEnemyHealthChangeEvent(AgentHealthChangeEvent evt)
        {
            if (evt.Agent != this)
                return;

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

                for (int i = 0; i < Renderer.Materials.Length; i++)
                    Renderer.Materials[i].color = Color.Lerp(_originColors[i], _hitColor, t);

                yield return null;
            }

            for (int i = 0; i < Renderer.Materials.Length; i++)
            {
                Renderer.Materials[i].color = _originColors[i];
            }
        }
    }
}
