using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using UnityEngine;

namespace JJH._02_Scripts.Systems.ParticleSystems
{
    public class BreathParticle : PoolableMono
    {
        public SoundClipSO BreathSound;

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