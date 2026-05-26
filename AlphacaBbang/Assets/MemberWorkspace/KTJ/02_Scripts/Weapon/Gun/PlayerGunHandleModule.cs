using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using System;
using UnityEngine;

public class PlayerGunHandleModule : WeaponHandleModule, IAfterInitModule
{
    [Header("Gun")]
    [SerializeField] private IWeapon firstSlotGun;
    [SerializeField] private IWeapon secondSlotGun;
    private WeaponSlotIndex currentGunIndex;
    //[SerializeField] private Gun TEST_GUN1;
    //[SerializeField] private Gun TEST_GUN2;

    [Header("System")]
    [SerializeField] private EventChannelSO gunChannel;
    [SerializeField] private EventChannelSO uiChannel;
    [field:SerializeField] public InventoryContainer InventoryContainer { get; private set; }

    public PlayerController PlayerController { get; private set; }

    [Inject] private Container _container;

    public override void Initialize(ModuleOwner owner)
    {
        base.Initialize(owner);
        PlayerController = owner as PlayerController;
    }

    public void AfterInitalize()
    {
        gunChannel.AddListener<WeaponEquipEvent>(HandleWeaponEquipEvent); // 총 지정
        gunChannel.AddListener<WeaponSlotEquipEvent>(HandleWeaponSlotEquipEvent); // 등에 다는거
        //gunChannel.AddListener<WeaponDropEvent>(HandleWeaponDropEvent);
    }

    private void OnDestroy()
    {
        gunChannel.RemoveListener<WeaponEquipEvent>(HandleWeaponEquipEvent);
        gunChannel.RemoveListener<WeaponSlotEquipEvent>(HandleWeaponSlotEquipEvent);
        //gunChannel.RemoveListener<WeaponDropEvent>(HandleWeaponDropEvent);
    }

    private void Start()
    {
        //// �׽�Ʈ �ڵ�
        //gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, WeaponSlotIndex.First, false));
        //gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, WeaponSlotIndex.First));
        //gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.First, false));
        // �������
    }

    //private void HandleWeaponDropEvent(WeaponDropEvent @event)
    //{
    //    Transform gunParent = gunHoldParent_1;
    //    switch (@event.Index)
    //    {
    //        case WeaponSlotIndex.First:
    //            gunParent = gunHoldParent_1;
    //            break;
    //        case WeaponSlotIndex.Second:
    //            gunParent = gunHoldParent_2;
    //            break;
    //    }
    //    if (gunParent.childCount > 0)
    //        Destroy(gunParent.GetChild(0).gameObject);
    //}

    public override void SetCurrentGun(IWeapon gun) // ������ �߻��� ���� ���Ѵ�. �� ���Կ��� 1,2�� �����ϸ� ���� �߻�� ������ ������.
    {
        base.SetCurrentGun(gun);
        gunChannel.RaiseEvent(GunEvents.WeaponEquipDataEvent.Init(CurrentWeapon.WeaponData));
    }



    private void HandleWeaponEquipEvent(WeaponEquipEvent @event)
    {
        switch (@event.SlotIndex)
        {
            case WeaponSlotIndex.First:
                if (firstSlotGun == null) return;
                SetCurrentGun(firstSlotGun);
                currentGunIndex = WeaponSlotIndex.First;
                //gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.Second, false));
                break;
            case WeaponSlotIndex.Second:
                if (secondSlotGun == null) return;
                SetCurrentGun(secondSlotGun);
                currentGunIndex = WeaponSlotIndex.Second;
                //gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.First, false));
                break;
        }
    }
    // ����
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
        // Removal case: if no gun provided or explicit unequip, just destroy existing child and clear cache
        if (@event.Gun == null || @event.IsEquip == false)
        {
            if (gunParent.childCount > 0)
                Destroy(gunParent.GetChild(0).gameObject);

            switch (@event.SlotIndex)
            {
                case WeaponSlotIndex.First: firstSlotGun = null; break;
                case WeaponSlotIndex.Second: secondSlotGun = null; break;
            }

            return;
        }

        if (gunParent.childCount > 0)
            Destroy(gunParent.GetChild(0).gameObject);

        GameObject gunObj = Instantiate(@event.Gun, gunParent);
        IWeapon gun = gunObj.GetComponent<IWeapon>();
        Debug.Assert(gun != null, "gun must implement IWeapon.");

        gunObj.transform.localPosition = Vector3.zero;
        gunObj.transform.localRotation = Quaternion.identity;
        gunObj.transform.localScale = Vector3.one;
        GameObjectInjector.InjectRecursive(gunObj, _container);

        // cache reference for equip
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
    public override void OnReloadStart()
    {
        base.OnReloadStart();
        uiChannel.RaiseEvent(UIEvents.BulletCountHandleEvent.Init(currentGunIndex, ("재장전중!")));
    }


    public override void OnCurrentBulletChanged(string current, string max)
    {
        uiChannel.RaiseEvent(UIEvents.BulletCountHandleEvent.Init(currentGunIndex, (current + "/" + max)));
    }
}