using MemberWorkspace.CHG._02_Scripts.TalkSystem;
using Reflex.Injectors;
using UnityEngine;

public class EnemyWeaponHandleModule : WeaponHandleModule, IEnemyWeaponModule
{
    [SerializeField] private BodyRecoilRotation BodyRecoilController;
    [SerializeField] private EnemyTalkSystem EnemyTalkSystem;

    public override void SetCurrentGun(IWeapon gun)
    {
        HandleWeaponSlotEquipEvent(gun);
    }


    public override void Initialize(ModuleOwner owner)
    {
        base.Initialize(owner);

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
    public override void OnReloadStart()
    {
        base.OnReloadStart();

    }
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
        Transform gunParent = gunHoldParent_2;

        if (gunParent.childCount > 0)
            Destroy(gunParent.GetChild(0).gameObject);

        MonoBehaviour gunMono = gun as MonoBehaviour;
        Debug.Assert(gunMono != null, "gun이 MonoBehaviour가 아닙니다.");

        GameObject gunObj = Instantiate(gunMono.gameObject, gunParent);

        gunObj.transform.localPosition = Vector3.zero;
        gunObj.transform.localRotation = Quaternion.identity;
        gunObj.transform.localScale = Vector3.one;

        IWeapon newGun = gunObj.GetComponent<IWeapon>();
        Debug.Assert(newGun != null, "복제된 gun이 IWeapon을 구현하지 않았습니다.");

        base.SetCurrentGun(newGun);
    }

    public override bool IsBulletInfinity()
    {
        return true;
    }
}