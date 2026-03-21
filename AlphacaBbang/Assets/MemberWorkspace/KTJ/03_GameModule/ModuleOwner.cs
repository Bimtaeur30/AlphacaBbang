using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ModuleOwner : MonoBehaviour
{
    protected Dictionary<Type, IModule> _moduleDict;

    protected virtual void Awake()
    {
        _moduleDict = GetComponentsInChildren<IModule>().ToDictionary(module => module.GetType());
        InitializeComponents();
        AfterInitializeComponents();
    }

    private void InitializeComponents()
    {
        foreach(IModule module in _moduleDict.Values)
        {
            module.Initialize(this);
        }
    }

    private void AfterInitializeComponents()
    {
        foreach (IAfterInitModule module in _moduleDict.Values.OfType<IAfterInitModule>())
        {
            module.AfterInitalize();
        }
    }

    public T GetModule<T>()
    {
        if (_moduleDict.TryGetValue(typeof(T), out var module)) // 빠른탐색
        {
            return (T)module;
        }

        IModule findModule = _moduleDict.Values.FirstOrDefault(moduleType => moduleType is T); // 실패 시 유연한 탐색

        if (findModule != null && findModule is T castedModule)
            return castedModule;

        return default;
    }
}
