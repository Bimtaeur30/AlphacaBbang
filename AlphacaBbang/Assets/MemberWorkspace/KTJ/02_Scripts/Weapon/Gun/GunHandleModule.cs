using UnityEngine;

public class GunHandleModule : MonoBehaviour, IModule
{
    [Header("State")]
    [field: SerializeField] public bool IsInputAim { get; private set; }
    [field: SerializeField] public bool IsInputFire { get; private set; }

    [Header("Gun")]
    [field: SerializeField] public Gun CurrentGun { get; private set; }

    public virtual void Initialize(ModuleOwner owner) { }

    public virtual void SetCurrentGun(Gun gun)
    {
        CurrentGun = gun;
        CurrentGun.Initialize(this);
        Debug.Assert(CurrentGun.GunDataSO != null, "CurrentGun.GunDataSO가 할당되지 않았습니다.");
    }

    private void Update()
    {
        if (CanFire() == false)
            return;
        if (CurrentGun == null)
            return;

        if (IsInputAim && IsInputFire)
        {
            CurrentGun.TickFire();
        }
    }

    public virtual void Fire(bool value)
    {
        IsInputFire = value;

        if (CanFire() == false)
            return;
        if (CurrentGun == null)
            return;

        if (value)
            CurrentGun.StartFire(IsInputAim);
        else
            CurrentGun.StopFire(IsInputAim);
    }

    public virtual void OnFire() { }

    // 장전 완료 콜백 — 기본은 아무것도 안 함 (플레이어: 다시 입력해야 발사)
    public virtual void OnReloadEnd() { }

    public void Aim(bool value)
    {
        IsInputAim = value;

        if (CurrentGun == null)
            return;

        if (!value && IsInputFire)
        {
            IsInputFire = false;
            CurrentGun.StopFire(false);
            return;
        }

        CurrentGun.SetAim(value);
    }

    protected virtual bool CanFire()
    {
        return true;
    }
}