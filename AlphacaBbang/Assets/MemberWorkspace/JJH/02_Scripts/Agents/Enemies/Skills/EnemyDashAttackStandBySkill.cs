using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.ParticleSystems;
using System.Collections;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDashAttackStandBySkill : MonoBehaviour, IEnemySkill
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO breathPoolItem;
        [SerializeField] private Transform breathTrans;
        [SerializeField] private float interval = 0.5f;

        private AbstractEnemy _owner;
        private Coroutine _breathCoroutine;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
        }

        public void UseSkill()
        {
            _breathCoroutine = StartCoroutine(BreathParticleCoroutine());
        }

        private IEnumerator BreathParticleCoroutine()
        {
            for (int i = 0; i < 3; i++)
            {
                BreathParticle effect = poolManager.Pop<BreathParticle>(breathPoolItem);

                effect.transform.position = breathTrans.position;
                effect.transform.rotation = breathTrans.rotation;

                effect.PlayBreathParticle();

                yield return new WaitForSeconds(interval);
            }

            _breathCoroutine = null;
        }
    }
}