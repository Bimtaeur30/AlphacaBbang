using UnityEngine;

public class WeaponHandleModule : MonoBehaviour, IModule
{
    public ModuleOwner Owner { get; private set; }
    [Header("State")]
    [field: SerializeField] public bool IsInputAim { get; private set; }
    [field: SerializeField] public bool IsInputFire { get; private set; }

    [Header("Gun")]
    [field: SerializeField] public IWeapon CurrentWeapon { get; private set; }

    public virtual void Initialize(ModuleOwner owner)
    {
        Owner = owner;
    }

    public virtual void SetCurrentGun(IWeapon gun)  
    {
        CurrentWeapon = gun;
        CurrentWeapon.Initialize(this);
        Debug.Assert(CurrentWeapon.WeaponData != null, "CurrentWeapon.WeaponData가 할당되지 않았습니다.");
    }

    private void Update()
    {
        if (CanFire() == false)
            return;
        if (CurrentWeapon == null)
            return;

        if (IsInputAim && IsInputFire)
        {
            CurrentWeapon.TickFire();
        }
    }

    public virtual void Fire(bool value)
    {
        IsInputFire = value;

        if (CanFire() == false)
            return;
        if (CurrentWeapon == null)
            return;

        if (value)
            CurrentWeapon.StartFire(IsInputAim);
        else
            CurrentWeapon.StopFire(IsInputAim);
    }

    public virtual void OnFire() { }

    // 장전 완료 콜백 — 기본은 아무것도 안 함 (플레이어: 다시 입력해야 발사)
    public virtual void OnReloadEnd() { }

    public void Aim(bool value)
    {
        IsInputAim = value;

        if (CurrentWeapon == null)
            return;

        if (!value && IsInputFire)
        {
            IsInputFire = false;
            CurrentWeapon.StopFire(false);
            return;
        }

        CurrentWeapon.SetAim(value);
    }

    protected virtual bool CanFire()
    {
        return true;
    }
}