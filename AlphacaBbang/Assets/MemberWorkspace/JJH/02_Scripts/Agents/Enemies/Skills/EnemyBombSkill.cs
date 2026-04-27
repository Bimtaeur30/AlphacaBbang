using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyBombSkill : MonoBehaviour, IEnemySkill
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO explosionPref;
        private AbstractEnemy _owner;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
        }

        public void UseSkill()
        {
            ExplosionPrefab effect = poolManager.Pop<ExplosionPrefab>(explosionPref);
            effect.Active(5f, _owner.EnemyData.Damage, _owner.Sensor.TargetLayer);
        }
    }
}