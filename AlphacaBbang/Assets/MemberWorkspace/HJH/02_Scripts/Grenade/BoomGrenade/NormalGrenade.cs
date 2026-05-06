using JJH._02_Scripts.Systems.ObjectPoolSystems;
using System.Collections;
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
        ExplosionPrefab effect = poolManager.Pop<ExplosionPrefab>(explosionPref);
        effect.Active(range, maxDamage, enemyLayer);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        float range = this.range;

        Gizmos.DrawWireSphere(gameObject.transform.position, range);
    }
}