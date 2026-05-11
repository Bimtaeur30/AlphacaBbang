using UnityEngine;

public class EnemyGunHandleModule : GunHandleModule, IWeapon
{
    [SerializeField] private BodyRecoilRotation BodyRecoilController;

    public override void SetCurrentGun(Gun gun)
    {
        base.SetCurrentGun(gun);
    }

    private void Start()
    {
        Init();
    }

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

    public override void OnFire()
    {
        base.OnFire();
        BodyRecoilController.ApplyRecoil(CurrentGun.GunDataSO.SpreadAngle);
    }

    // 장전 완료 시 자동으로 발사 재개 (적은 항상 조준 상태이므로)
    public override void OnReloadEnd()
    {
        if (IsInputAim)
        {
            // IsInputFire를 true로 되돌리고 발사 재개
            Fire(true);
        }
    }

    protected override bool CanFire()
    {
        if (CurrentGun.Magazine.IsReloading) return false;
        return true;
    }
}