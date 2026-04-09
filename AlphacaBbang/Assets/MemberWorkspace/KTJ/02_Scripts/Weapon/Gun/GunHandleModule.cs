using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GunHandleModule : MonoBehaviour, IModule, IWeapon
{
    [Header("State")]
    [field: SerializeField] public bool OnAim { get; private set; }
    [field: SerializeField] public bool OnFire { get; private set; }

    [Header("Gun")]
    [field: SerializeField] public Gun CurrentGun { get; private set; }

    [Header("System")]
    [SerializeField] private EventChannelSO systemChannel;

    public void Initialize(ModuleOwner owner)
    {
        Debug.Log("GunHandleModule Initialize");

        Debug.Assert(CurrentGun != null, "CurrentGun이 할당되지 않았습니다.");
        Debug.Assert(CurrentGun.GunDataSO != null, "CurrentGun.GunDataSO가 할당되지 않았습니다.");

        systemChannel.RaiseEvent(SystemEventChannel.WeaponEqnupEventChannel.Init(CurrentGun.GunDataSO));
        CurrentGun.Initialize();
    }

    private void Update()
    {
        if (CurrentGun == null)
            return;

        if (OnAim && OnFire)
        {
            CurrentGun.TickFire();
        }
    }

    public void Fire(bool value)
    {
        OnFire = value;

        if (CurrentGun == null)
            return;

        if (value)
        {
            CurrentGun.StartFire(OnAim);
        }
        else
        {
            CurrentGun.StopFire(OnAim);
        }
    }

    public void Aim(bool value)
    {
        OnAim = value;

        if (CurrentGun == null)
            return;

        CurrentGun.SetAim(value);
    }

    // 이하 적들이 사용하는 코드

    public void Attack(Vector3 vector, bool val)
    {
        Debug.Assert(CurrentGun != null, "현재 장착중인 총이 없습니다.");
        Fire(val);
    }
    public void SetAim(bool val)
    {
        Aim(val);
    }
    public void Init()
    {
        SetAim(true);
    }

    private void Start()
    {
        Init();
    }

}