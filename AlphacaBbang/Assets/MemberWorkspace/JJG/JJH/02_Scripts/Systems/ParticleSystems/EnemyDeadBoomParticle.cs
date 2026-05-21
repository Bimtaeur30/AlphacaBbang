using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

namespace JJH._02_Scripts.Systems.ParticleSystems
{
    public class EnemyDeadBoomParticle : PoolableMono
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleSystemStopped()
        {
            PoolManager.Push(this);
        }

        public void PlayBoomParticle()
        {
            _particleSystem.Play();
        }
    }
}