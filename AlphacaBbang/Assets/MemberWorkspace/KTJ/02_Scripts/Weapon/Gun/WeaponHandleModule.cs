using System.Globalization;
using UnityEngine;

public class WeaponHandleModule : MonoBehaviour, IModule
{
    public ModuleOwner Owner { get; private set; }

    [Header("State")]
    [field: SerializeField] public bool IsInputAim { get; private set; }
    [field: SerializeField] public bool IsInputFire { get; private set; }

    [Header("Gun")]
    [field: SerializeField] public IWeapon CurrentWeapon { get; private set; }
    [field: SerializeField] public LayerMask TargetLayer { get; private set; }

    [Header("Transform")]
    [SerializeField] protected Transform gunHoldParent_1;
    [SerializeField] protected Transform gunHoldParent_2;

    private bool _pendingFireStart = false;
    private bool _pendingFireStop = false;

    public virtual void Initialize(ModuleOwner owner)
    {
        Owner = owner;
    }

    public virtual void SetCurrentGun(IWeapon gun)
    {
        CurrentWeapon = gun;
        CurrentWeapon.Initialize(this);
        if (CurrentWeapon.WeaponData != null)
            Debug.Assert(CurrentWeapon.WeaponData != null, "CurrentWeapon.WeaponData가 할당되지 않았습니다.");
    }

    private void LateUpdate()
    {
        if (CanFire() && CurrentWeapon != null)
        {
            if (_pendingFireStop)
            {
                CurrentWeapon.StopFire(IsInputAim);
                _pendingFireStop = false;
            }

            if (_pendingFireStart)
            {
                CurrentWeapon.StartFire(IsInputAim);
                _pendingFireStart = false;
            }

            if (IsInputAim && IsInputFire)
            {
                CurrentWeapon.TickFire();
            }
        }
        else
        {
            _pendingFireStart = false;
            _pendingFireStop = false;
        }
    }

    public virtual void Fire(bool value)
    {
        IsInputFire = value;

        if (value)
            _pendingFireStart = true;
        else
            _pendingFireStop = true;
    }

    public virtual void OnFire() { }

    public virtual void OnReloadStart() { }

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

    public virtual bool IsBulletInfinity()
    {
        return false;
    }

    public virtual void OnCurrentBulletChanged(string current, string max) { }
}