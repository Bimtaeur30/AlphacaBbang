using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDeadSkill : MonoBehaviour, IEnemySkill
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO deadBoomPref;

        private AbstractEnemy _owner;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
        }

        public void UseSkill()
        {
            EnemyDeadBoomParticle effect = poolManager.Pop<EnemyDeadBoomParticle>(deadBoomPref);
            effect.transform.position = _owner.transform.position;
            effect.PlayBoomParticle();
        }
    }
}