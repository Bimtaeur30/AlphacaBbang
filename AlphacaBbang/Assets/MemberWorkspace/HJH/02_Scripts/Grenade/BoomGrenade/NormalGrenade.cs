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
        if (poolManager == null) { Debug.LogError("poolManager null - Normal"); return; }
        if (explosionPref == null) { Debug.LogError("explosionPref null - Normal"); return; }

        ExplosionPrefab effect = poolManager.Pop<ExplosionPrefab>(explosionPref);
        if (effect == null) { Debug.LogError("Pool에서 ExplosionPrefab 못가져옴 - Normal"); return; }

        effect.Active(range, maxDamage, enemyLayer);
        effect.gameObject.transform.position = transform.position;
    }
}