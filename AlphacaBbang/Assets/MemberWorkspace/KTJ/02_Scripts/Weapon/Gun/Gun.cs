using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunRenderer Renderer { get; private set; }
    [field:SerializeField] public GunDataSO GunDataSO { get; private set; }
    [field:SerializeField] public LayerMask TargetLayer { get; private set; }

    public abstract void Fire();

    protected virtual void Awake()
    {
        Renderer = GetComponentInChildren<GunRenderer>();
        Debug.Assert(Renderer != null, "GunRenderer가 자식으로 붙어있지 않습니다.");
    }
}
