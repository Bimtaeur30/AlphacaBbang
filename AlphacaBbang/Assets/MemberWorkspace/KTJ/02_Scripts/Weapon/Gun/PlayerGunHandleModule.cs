using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
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


    public PlayerController PlayerController { get; private set; }

    [Inject] private Container _container;

    public override void Initialize(ModuleOwner owner)
    {
        base.Initialize(owner);
        PlayerController = owner as PlayerController;
    }

    public void AfterInitalize()
    {
        gunChannel.AddListener<WeaponEquipEvent>(HandleWeaponEquipEvent);
        gunChannel.AddListener<WeaponSlotEquipEvent>(HandleWeaponSlotEquipEvent);
    }

    private void OnDestroy()
    {
        gunChannel.RemoveListener<WeaponEquipEvent>(HandleWeaponEquipEvent);
        gunChannel.RemoveListener<WeaponSlotEquipEvent>(HandleWeaponSlotEquipEvent);
    }
    
    private void Start()
    {
        //// 테스트 코드
        //gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(null, WeaponSlotIndex.First, false));
        ////gunChannel.RaiseEvent(GunEvents.WeaponSlotEquipEvent.Init(TEST_GUN2, WeaponSlotIndex.Second));
        //gunChannel.RaiseEvent(GunEvents.WeaponEquipEvent.Init(WeaponSlotIndex.First, false));
        // 여기까지
    }

    public override void SetCurrentGun(IWeapon gun) // 실제로 발사할 총을 정한다. 퀵 슬롯에서 1,2번 선택하면 현재 발사될 총으로 지정됨.
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
                break;
            case WeaponSlotIndex.Second:
                if (secondSlotGun == null) return;
                SetCurrentGun(secondSlotGun);
                currentGunIndex = WeaponSlotIndex.Second;
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
        // IWeapon gun = Instantiate(@event.Gun, gunParent);
        

        GameObject gunObj = Instantiate(@event.Gun, gunParent);
        IWeapon gun = gunObj.GetComponent<IWeapon>();
        Debug.Assert(gun != null, "gun이 IWeapon을 구현하지 않았습니다.");

        gunObj.transform.localPosition = Vector3.zero;
        gunObj.transform.localRotation = Quaternion.identity;
        gunObj.transform.localScale = Vector3.one;
        GameObjectInjector.InjectRecursive(gunObj, _container);

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

    public override void OnCurrentBulletChanged(string current, string max)
    {
        uiChannel.RaiseEvent(UIEvents.BulletCountHandleEvent.Init(currentGunIndex, (current + "/" + max)));
    }
}