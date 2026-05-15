using JJH._02_Scripts.Systems.ObjectPoolSystems;
using System.Collections;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDashAttackStandBySkill : MonoBehaviour, IEnemySkill
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO breathPoolItem;
        [SerializeField] private Transform breathTrans;

        private AbstractEnemy _owner;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
        }

        public void UseSkill()
        {
            for (int i = 0; i < 3; i++)
            {
                BreathParticle effect = poolManager.Pop<BreathParticle>(breathPoolItem);
                effect.gameObject.transform.position = breathTrans.position;
                effect.PlayBreathParticle();
            }
        }

        private IEnumerator BreathParticle()
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}