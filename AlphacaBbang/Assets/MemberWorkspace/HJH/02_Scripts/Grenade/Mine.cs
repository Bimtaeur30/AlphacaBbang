using JJH._02_Scripts.Systems.ObjectPoolSystems;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private PoolManagerSO poolManager;
    [SerializeField] private PoolItemSO explosionPref;

    [SerializeField] private LayerMask bombLayerMasks;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("¤±");
            ExplosionPrefab effect = poolManager.Pop<ExplosionPrefab>(explosionPref);
            effect.Active(5f, 25f, bombLayerMasks);
            Destroy(gameObject);
        }
    }
}
