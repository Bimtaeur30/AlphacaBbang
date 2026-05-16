using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyBombSkill : MonoBehaviour, IEnemySkill
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO explosionPref;
        [SerializeField] private float explosionDamage = 30f;

        private AbstractEnemy _owner;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
        }

        public void UseSkill()
        {
            ExplosionPrefab effect = poolManager.Pop<ExplosionPrefab>(explosionPref);
            effect.transform.position = _owner.transform.position;
            effect.Active(5f, explosionDamage, _owner.Sensor.TargetLayer);
        }
    }
}