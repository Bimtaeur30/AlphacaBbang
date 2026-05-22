using System.Collections.Generic;
using UnityEngine;

namespace JJH._02_Scripts.Systems.ObjectPoolSystems
{
    public class Pool
    {
        private readonly Stack<IPoolable> _pool;
        private readonly Transform _parent;
        private readonly GameObject _prefab;

        public Pool(IPoolable poolable, Transform parent, int count)
        {
            _pool = new Stack<IPoolable>(count);
            _parent = parent;
            _prefab = poolable.GameObject;
            for (int i = 0; i < count; i++)
            {
                GameObject gameObj = Object.Instantiate(_prefab, _parent);
                gameObj.SetActive(false);
                IPoolable item = gameObj.GetComponent<IPoolable>();
                Debug.Assert(item != null, $"Poolable component not found on {_prefab.name}");
                _pool.Push(item);
            }
        }

        public IPoolable Pop()
        {
            IPoolable item;
            if (_pool.Count == 0)
            {
                GameObject gameObj = Object.Instantiate(_prefab, _parent);
                item = gameObj.GetComponent<IPoolable>();
                Debug.Assert(item != null, $"Poolable component not found on {_prefab.name}");
            }
            else
            {
                item = _pool.Pop();
                item.GameObject.SetActive(true);
            }
            item.ResetItem();
            return item;
        }

        public void Push(IPoolable item)
        {
            item.GameObject.SetActive(false);
            _pool.Push(item);
        }
    }
}