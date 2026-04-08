using JJH._02_Scripts.Systems.ObjectPoolSystems;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PoolParticleEffect : PoolableMono
{
    private ParticleSystem _particleSystem;
    [SerializeField] private PoolManagerSO poolManager;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void PlayClipEffect(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        _particleSystem.Play();
    }

    private void OnParticleSystemStopped()
    {
        if (_particleSystem != null)
            poolManager.Push(this);
    }
}
