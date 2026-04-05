using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

public class ShotGun : Gun
{
    protected override void FireInternal()
    {
        Vector3 origin = firePos.position;
        Vector3 baseDirection = GetShootDirection();

        for (int i = 0; i < GunDataSO.BulletFireCount; i++)
        {
            float spreadAngleX = Random.Range(-GunDataSO.SpreadAngle, GunDataSO.SpreadAngle);
            float spreadAngleY = Random.Range(-GunDataSO.SpreadAngle, GunDataSO.SpreadAngle);
            
            Quaternion spreadRotation = Quaternion.Euler(spreadAngleX, spreadAngleY, 0);
            Vector3 direction = (spreadRotation * baseDirection).normalized;

            Vector3 endPoint = origin + direction * rayDistance;

            OnFire(origin, direction);

            Debug.DrawRay(origin, direction * rayDistance, Color.red, 0.2f);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, TargetLayer))
            {
                Debug.Log("户具!");
                endPoint = hit.point;

                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                    Debug.Log("圈费费ぉぉぉぉ");
                }

                OnHit(hit);
            }
            else
            {
                OnMiss(origin, endPoint);
            }

            if (lineRenderer != null)
            {
                base.DrawLine(origin, endPoint, 0.05f);
            }
        }
    }

    protected override void OnFire(Vector3 origin, Vector3 direction)
    {
    }

    protected override void OnHit(RaycastHit hit)
    {
        if (poolManager == null || bulletParticle == null)
            return;

        PoolParticleEffect effect = poolManager.Pop<PoolParticleEffect>(bulletParticle);
        effect.PlayClipEffect(hit.point, Quaternion.LookRotation(hit.normal));
    }

    protected override void OnMiss(Vector3 origin, Vector3 endPoint)
    {
    }
}
