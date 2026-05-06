using JJH._02_Scripts_Systems.EventSystems;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerGunHandleModule : GunHandleModule, IAfterInitModule
{
    [Header("Gun")]
    [SerializeField] private Gun firstSlotGun;
    [SerializeField] private Gun secondSlotGun;
    [SerializeField] private Gun TEST_GUN1;
    [SerializeField] private Gun TEST_GUN2;

    [Header("Transform")]
    [SerializeField] private Transform gunHoldParent_1;
    [SerializeField] private Transform gunHoldParent_2;

    [Header("System")]
    [SerializeField] private EventChannelSO systemChannel;

    public PlayerController PlayerController { get; private set; }

    public override void Initialize(ModuleOwner owner)
    {
        base.Initialize(owner);
        PlayerController = owner as PlayerController;
    }

    public void AfterInitalize()
    {
        systemChannel.AddListener<WeaponEquipEvent>(HandleWeaponEquipEvent);
        systemChannel.AddListener<WeaponSlotEquipEvent>(HandleWeaponSlotEquipEvent);
    }

    private void Start()
    {
        // 테스트 코드
        systemChannel.RaiseEvent(SystemEventChannel_TJ.WeaponSlotEquipEvent.Init(TEST_GUN1, WeaponSlotIndex.First));
        systemChannel.RaiseEvent(SystemEventChannel_TJ.WeaponSlotEquipEvent.Init(TEST_GUN2, WeaponSlotIndex.Second));
        systemChannel.RaiseEvent(SystemEventChannel_TJ.WeaponEquipEvent.Init(WeaponSlotIndex.First));
        // 여기까지
    }

    protected override void SetCurrentGun(Gun gun)
    {
        base.SetCurrentGun(gun);
        systemChannel.RaiseEvent(SystemEventChannel_TJ.WeaponEquipDataEvent.Init(CurrentGun.GunDataSO));
    }

    private void HandleWeaponEquipEvent(WeaponEquipEvent @event)
    {
        switch (@event.SlotIndex)
        {
            case WeaponSlotIndex.First:
                if (firstSlotGun == null) return;
                SetCurrentGun(firstSlotGun);
                break;
            case WeaponSlotIndex.Second:
                if (secondSlotGun == null) return;
                SetCurrentGun(secondSlotGun);
                break;
        }
    }
    // 수정
    private void HandleWeaponSlotEquipEvent(WeaponSlotEquipEvent @event)
    {
        Transform gunParent = gunHoldParent_1;
        switch (@event.SlotIndex)
        {
            case WeaponSlotIndex.First:
                gunParent = gunHoldParent_1;
                break;
            case WeaponSlotIndex.Second:
                gunParent = gunHoldParent_2;
                break;
        }

        if (gunParent.childCount > 0)
            Destroy(gunParent.GetChild(0).gameObject);

        Gun gun = Instantiate(@event.Gun, gunParent);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        gun.transform.localScale = Vector3.one;

        // 인스턴스를 슬롯에 저장
        switch (@event.SlotIndex)
        {
            case WeaponSlotIndex.First: firstSlotGun = gun; break;
            case WeaponSlotIndex.Second: secondSlotGun = gun; break;
        }
    }

    protected override bool CanFire()
    {
        return PlayerController.IsPureAiming;
    }
}