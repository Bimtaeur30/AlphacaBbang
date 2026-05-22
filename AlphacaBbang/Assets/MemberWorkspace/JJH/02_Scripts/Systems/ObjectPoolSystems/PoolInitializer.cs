using UnityEngine;

namespace JJH._02_Scripts.Systems.ObjectPoolSystems
{
    public class PoolInitializer : MonoBehaviour
    {
        [field: SerializeField] public PoolManagerSO PoolManager { get; private set; }

        private void Awake()
        {
            PoolInitializer[] initializers = FindObjectsByType<PoolInitializer>(FindObjectsSortMode.None);
            if (initializers.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            PoolManager.InitializePool(transform);
            DontDestroyOnLoad(gameObject);
        }

        public T Pop<T>(PoolItemSO item) where T : IPoolable
        {
            return PoolManager.Pop<T>(item);
        }

        public void Push(IPoolable target)
        {
            PoolManager.Push(target);
        }
    }
}