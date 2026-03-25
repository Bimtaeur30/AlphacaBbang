using UnityEngine;

namespace JJH._02_Scripts.Systems.ObjectPoolSystems
{
    public interface IPoolable
    {
        public PoolItemSO PoolType { get; set; }
        public GameObject GameObject { get; }
        public void ResetItem();
    }
}
