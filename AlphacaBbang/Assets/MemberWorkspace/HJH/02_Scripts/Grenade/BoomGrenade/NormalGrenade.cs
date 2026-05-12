using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

public class NormalGrenade : GrenadeBehavior
{
    [SerializeField] private PoolManagerSO poolManager;
    [SerializeField] private PoolItemSO explosionPref;
    [SerializeField] private LayerMask enemyLayer;

    public float range;
    public float maxDamage;

    protected override void OnExplode()
    {
        if (poolManager == null)
        {
            Debug.LogError("poolManager가 null입니다. Inspector에서 할당하세요. - Normal");
            return;
        }

        if (explosionPref == null)
        {
            Debug.LogError("explosionPref가 null입니다. Inspector에서 할당하세요. - Normal");
            return;
        }

        ExplosionPrefab effect = poolManager.Pop<ExplosionPrefab>(explosionPref);

        if (effect == null)
        {
            Debug.LogError("Pool에서 ExplosionPrefab을 가져오지 못했습니다. - Normal");
            return;
        }

        effect.Active(range, maxDamage, enemyLayer);
        effect.gameObject.transform.position = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}