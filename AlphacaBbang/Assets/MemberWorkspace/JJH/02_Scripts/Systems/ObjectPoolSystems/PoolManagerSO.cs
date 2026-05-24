using System.Collections.Generic;
using UnityEngine;

namespace JJH._02_Scripts.Systems.ObjectPoolSystems
{
    [CreateAssetMenu(fileName = "PoolManager", menuName = "Pool/PoolManager", order = 5)]
    public class PoolManagerSO : ScriptableObject
    {
        public List<PoolItemSO> itemList = new();

        private Dictionary<PoolItemSO, Pool> _pools;
        private Transform _rootTrm;

        public void InitializePool(Transform root)
        {
            _rootTrm = root;
            _pools = new Dictionary<PoolItemSO, Pool>();

            foreach (var item in itemList)
            {
                IPoolable poolable = item.prefab.GetComponent<IPoolable>();
                Debug.Assert(poolable != null, $"PoolItem does not have IPoolable {item.prefab.name}");

                var pool = new Pool(poolable, _rootTrm, item.initCount);

                _pools.Add(item, pool);
            }
        }

        public T Pop<T>(PoolItemSO type) where T : IPoolable
        {
            Debug.Assert(_rootTrm != null, "Pool must be initialized before use");

                if (_pools.TryGetValue(type, out Pool pool))
            {
                return (T)pool.Pop();
            }
            return default;
        }

        public void Push(IPoolable item)
        {
            Debug.Assert(_rootTrm != null, "Pool must be initialized before use");
            if (_pools.TryGetValue(item.PoolType, out Pool pool))
            {
                pool.Push(item);
            }
        }
    }
}