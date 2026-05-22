using UnityEngine;

namespace JJH._02_Scripts.Systems.ObjectPoolSystems
{
    [CreateAssetMenu(fileName = "Pool Item", menuName = "Bbang/SO/Pool/Pool Item", order = 0)]
    public class PoolItemSO : ScriptableObject
    {
        [HideInInspector] public string poolingName;
        public GameObject prefab;
        public int initCount;

        private void OnValidate()
        {
            if (prefab != null && !prefab.TryGetComponent(out IPoolable _))
            {
                Debug.LogError($"Poolable component not found on {prefab.name}");
                prefab = null;
            }
        }
    }
}