using UnityEngine;

public class BossWeaponHandleModule : WeaponHandleModule, IEnemyWeaponModule
{
    [SerializeField] private BodyRecoilRotation BodyRecoilController;

    public override void SetCurrentGun(IWeapon gun)
    {
        base.SetCurrentGun(gun);
        HandleWeaponSlotEquipEvent(gun);
    }

    private void Start()
    {
        Init();
    }

    public void Attack(Vector3 vector, bool val)
    {
        Debug.Assert(CurrentWeapon != null, "현재 장착중인 총이 없습니다.");
        Fire(val);
    }

    public void SetAim(bool val)
    {
        Aim(val);
    }

    public void Init()
    {
        //SetAim(true);
    }

    public override void OnFire()
    {
        base.OnFire();
        BodyRecoilController.ApplyRecoil(CurrentWeapon.WeaponData.SpreadAngle);
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
    private void HandleWeaponSlotEquipEvent(IWeapon gun)
    {
        if (gunHoldParent_1.childCount > 0)
            Destroy(gunHoldParent_1.GetChild(0).gameObject);
        if (gunHoldParent_2.childCount > 0)
            Destroy(gunHoldParent_2.GetChild(0).gameObject);

        GameObject gunObj_1 = Instantiate((gun as MonoBehaviour).gameObject, gunHoldParent_1);
        GameObject gunObj_2= Instantiate((gun as MonoBehaviour).gameObject, gunHoldParent_2);
        Debug.Assert(gun != null, "gun이 IWeapon을 구현하지 않았습니다.");

        gunObj_1.transform.localPosition = Vector3.zero;
        gunObj_1.transform.localRotation = Quaternion.identity;
        gunObj_1.transform.localScale = Vector3.one;
        gunObj_2.transform.localPosition = Vector3.zero;
        gunObj_2.transform.localRotation = Quaternion.identity;
        gunObj_2.transform.localScale = Vector3.one;
    }
}
