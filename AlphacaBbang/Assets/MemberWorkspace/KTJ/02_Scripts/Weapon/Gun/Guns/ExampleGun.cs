using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

public class ExampleGun : Gun
{

    protected override void OnFire(Vector3 origin, Vector3 direction)
    {
        // 필요하면 여기서 총구 화염, 탄피 배출, 사운드 같은 거 처리
        // 예:
        // PlayMuzzleFlash();
        // PlayFireSound();
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