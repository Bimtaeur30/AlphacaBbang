using UnityEngine;

namespace JJH._02_Scripts.Systems.ObjectPoolSystems
{
    public abstract class PoolableMono : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolManagerSO PoolManager { get; set; }
        [field: SerializeField] public PoolItemSO PoolType { get; set; }
        public GameObject GameObject => this != null ? gameObject : null;
        public virtual void ResetItem() { }
    }
}