using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class BreathParticle : PoolableMono
    {
        [SerializeField] private PoolManagerSO poolManager;

        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleSystemStopped()
        {
            poolManager.Push(this);
        }

        public void PlayBreathParticle()
        {
            _particleSystem.Play();
        }
    }
}