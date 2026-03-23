using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [field:SerializeField] public GunDataSO GunDataSO { get; private set; }
    [field:SerializeField] public LayerMask TargetLayer { get; private set; }

    public abstract void Fire();
}
