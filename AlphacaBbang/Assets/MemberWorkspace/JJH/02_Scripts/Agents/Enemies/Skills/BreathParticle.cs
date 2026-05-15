using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class BreathParticle : PoolableMono
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

        public void PlayBreathParticle()
        {
            _particleSystem.Play();
        }
    }
}