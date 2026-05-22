using Reflex.Injectors;
using UnityEngine;

public class EnemyWeaponHandleModule : WeaponHandleModule, IEnemyWeaponModule
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
        Transform gunParent = gunHoldParent_1;

        if (gunParent.childCount > 0)
            Destroy(gunParent.GetChild(0).gameObject);
        // IWeapon gun = Instantiate(@event.Gun, gunParent);


        GameObject gunObj = Instantiate((gun as MonoBehaviour).gameObject, gunParent);
        Debug.Assert(gun != null, "gun이 IWeapon을 구현하지 않았습니다.");

        gunObj.transform.localPosition = Vector3.zero;
        gunObj.transform.localRotation = Quaternion.identity;
        gunObj.transform.localScale = Vector3.one;
    }

}