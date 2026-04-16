using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GunHandleModule : MonoBehaviour, IModule
{
    [Header("State")]
    [field: SerializeField] public bool IsAim { get; private set; }
    [field: SerializeField] public bool IsFire { get; private set; } // 이거 False로 만들어야함 장전하는동안

    [Header("Gun")]
    [field: SerializeField] public Gun CurrentGun { get; private set; }

    [Header("System")]
    [SerializeField] private EventChannelSO systemChannel;


    public void Initialize(ModuleOwner owner)
    {
        Debug.Log($"{gameObject.name} / {GetType().Name} Initialize");

        Debug.Assert(CurrentGun != null, "CurrentGun이 할당되지 않았습니다.");
        Debug.Assert(CurrentGun.GunDataSO != null, "CurrentGun.GunDataSO가 할당되지 않았습니다.");

        systemChannel.RaiseEvent(SystemEventChannel.WeaponEqnupEventChannel.Init(CurrentGun.GunDataSO));
        CurrentGun.Initialize(this);

        Debug.Log($"{gameObject.name} / {GetType().Name} 초기화 완료");
    }


    private void Update()
    {
        if (CurrentGun == null)
            return;

        if (IsAim && IsFire)
        {
            CurrentGun.TickFire();
        }

        //if (CurrentGun.Magazine.IsReloading)
        //{
        //    CurrentGun.StopFire(true);
        //}
    }

    public virtual void Fire(bool value)
    {
        IsFire = value;

        if (CurrentGun == null)
            return;

        if (value)
        {
            CurrentGun.StartFire(IsAim);
        }
        else
        {
            CurrentGun.StopFire(IsAim);
        }
    }
    public virtual void OnFire() { }

    public void Aim(bool value)
    {
        IsAim = value;

        if (CurrentGun == null)
            return;

        CurrentGun.SetAim(value);
    }
}