using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ExplosionPrefab : PoolableMono
{
    [SerializeField] private EventChannelSO SoundChannel;
    [SerializeField] private SoundClipSO _explosionSound;
    [SerializeField] private ParticleSystem _particleSystem;
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Active(float range, float maxDamage, LayerMask layerMask)
    {
        Vector3 particleScale = _particleSystem.gameObject.transform.localScale;
        Vector3 multiplyScale = particleScale * range;
        _particleSystem.gameObject.transform.localScale = multiplyScale;

        _particleSystem.Play();
        SoundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(_explosionSound, this.transform));
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            range,
            layerMask,
            QueryTriggerInteraction.Ignore
        );
        foreach (var hit in hits)
        {
            Vector3 closestPoint = hit.ClosestPoint(transform.position);
            float distance = Vector3.Distance(transform.position, closestPoint);

            float t = Mathf.Clamp01(distance / range);
            float damage = maxDamage * (1f - t);

            if (hit.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damage);

        }

        ImpulseScreenShake(range / 10);
    }

    private void ImpulseScreenShake(float power)
    {
        _impulseSource.GenerateImpulse(new Vector3(0, power, 0));
    }

    private void OnParticleSystemStopped()
    {
        PoolManager.Push(this);
    }
}
