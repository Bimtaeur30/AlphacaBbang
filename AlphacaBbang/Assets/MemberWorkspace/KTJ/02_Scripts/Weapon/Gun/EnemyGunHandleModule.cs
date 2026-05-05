using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class EnemyGunHandleModule : GunHandleModule, IWeapon
{
    [SerializeField] private BodyRecoilRotation BodyRecoilController;

    private void Awake()
    {
        Debug.Assert(BodyRecoilController != null, "에너미 건 모듈에는 BodyRecoilRotation 컴포넌트가 붙은 오브젝트가 존재해야합니다.");
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

    private void Start()
    {
        Init();
    }

    public override void OnFire()
    {
        base.OnFire();
        BodyRecoilController.ApplyRecoil(CurrentGun.GunDataSO.SpreadAngle);
    }

    protected override bool CanFire()
    {
        return true;
    }
}
