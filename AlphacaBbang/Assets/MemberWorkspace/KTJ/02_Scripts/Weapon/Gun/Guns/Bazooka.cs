using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Bazooka : Gun
{
    [SerializeField] private PoolItemSO warHeadSO;
    [SerializeField] private ParticleSystem smokeParticle;

    protected override void FireInternal()
    {
        Warhead warHead = poolManager.Pop<Warhead>(warHeadSO);
        Vector3 origin = firePos.transform.position;
        Vector3 direction = GetShootDirection(); // return transform.right.normalized;
        Vector3 endPoint = origin + direction * rayDistance;
        endPoint.y = 0;

        warHead.transform.SetPositionAndRotation(
            origin,
            Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0)
        );

        warHead.Fire(direction, 30f);

        OnFire(origin, direction);
    }

    protected override void OnFire(Vector3 origin, Vector3 direction)
    {
        smokeParticle.Play();
    }

    protected override void OnHit(RaycastHit hit)
    {
        if (poolManager == null || bulletParticle == null)
            return;
    }

    protected override void OnMiss(Vector3 origin, Vector3 endPoint)
    {
    }
}