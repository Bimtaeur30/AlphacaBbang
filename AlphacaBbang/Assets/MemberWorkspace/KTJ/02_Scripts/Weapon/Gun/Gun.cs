using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [field:SerializeField] public GunData GunData { get; private set; }

    public abstract void Fire();
}
