using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CrossHairModule : MonoBehaviour, IModule
{
    public Vector2 CHMousePos { get; private set; }

    [SerializeField] private Image crossHairImg;
    [SerializeField] private EventChannelSO SystemChannel;

    [Header("CrossHair Settings")]
    [SerializeField] private float followSpeed = 20f;
    [SerializeField] private float recoilDistance = 60f;
    [SerializeField] private float recoilRecoverSpeed = 10f;

    private bool isCrossHairActive;
    private Player_TJ player;

    private Vector2 mousePos;
    private Vector2 recoilOffset;

    // onFire 동안 FireInterval마다 반동 적용하기 위한 타이머
    private float fireRecoilTimer;

    public void Initialize(ModuleOwner owner)
    {
        player = owner as Player_TJ;
        Debug.Assert(player != null, "CrossHairModule : player is null");
        Debug.Assert(crossHairImg != null, "CrossHairModule : crossHairImg is null");
        Debug.Assert(SystemChannel != null, "CrossHairModule : SystemChannel is null");
    }

    private void OnEnable()
    {
        SystemChannel.AddListener<WeaponEquipEvent>(OnWeaponEquip);
        SystemChannel.AddListener<WeaponDropEvent>(OnWeaponDrop);
    }

    private void OnDisable()
    {
        SystemChannel.RemoveListener<WeaponEquipEvent>(OnWeaponEquip);
        SystemChannel.RemoveListener<WeaponDropEvent>(OnWeaponDrop);
    }

    private void Update()
    {
        if (!isCrossHairActive || crossHairImg == null || Mouse.current == null || player == null)
            return;

        mousePos = Mouse.current.position.ReadValue();

        bool isFiring = player.gunHandleModule != null && player.gunHandleModule.onFire && player.gunHandleModule.onAim;

        if (isFiring && player.gunHandleModule.isAuto)
        {
            HandleRepeatedFireRecoil();
        }
        else if (isFiring)
        {
            // 자동이 아닌 무기는 발사할 때마다 즉시 반동 적용
            if (fireRecoilTimer <= 0f)
            {
                ApplyRecoil(player.gunHandleModule.CurrentGun.GunDataSO);
                fireRecoilTimer = float.MaxValue; // 다음 발사 전까지 반동 타이머 무한 대기
            }
        }
        else
        {
            // 발사 멈추면 다음 발사 때 즉시 튀도록 초기화
            fireRecoilTimer = 0f;
        }

        // 반동 오프셋은 서서히 원위치로 복귀
        recoilOffset = Vector2.Lerp(recoilOffset, Vector2.zero, Time.deltaTime * recoilRecoverSpeed);

        Vector2 targetScreenPos = mousePos + recoilOffset;

        crossHairImg.rectTransform.position = Vector2.Lerp(
            crossHairImg.rectTransform.position,
            targetScreenPos,
            Time.deltaTime * followSpeed
        );

        CHMousePos = crossHairImg.rectTransform.position;
    }

    private void HandleRepeatedFireRecoil()
    {
        if (player.gunHandleModule == null || player.gunHandleModule.CurrentGun == null)
            return;

        GunDataSO gunData = player.gunHandleModule.CurrentGun.GunDataSO;
        if (gunData == null)
            return;

        float fireInterval = gunData.FireInterval;

        // 잘못된 값 방어
        if (fireInterval <= 0f)
        {
            ApplyRecoil(gunData);
            return;
        }

        // 타이머가 0 이하이면 즉시 반동 적용
        if (fireRecoilTimer <= 0f)
        {
            ApplyRecoil(gunData);
            fireRecoilTimer = fireInterval;
        }

        fireRecoilTimer -= Time.deltaTime;
    }

    private void ApplyRecoil(GunDataSO gunData)
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector2 playerScreenPos = cam.WorldToScreenPoint(player.transform.position);

        Vector2 aimDir = mousePos - playerScreenPos;
        if (aimDir.sqrMagnitude < 0.0001f)
            aimDir = Vector2.up;
        else
            aimDir.Normalize();

        float randomAngle = Random.Range(-gunData.SpreadAngle, gunData.SpreadAngle);
        Vector2 recoilDir = RotateVector(aimDir, randomAngle);

        recoilOffset += recoilDir * recoilDistance;
    }

    private Vector2 RotateVector(Vector2 dir, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            dir.x * cos - dir.y * sin,
            dir.x * sin + dir.y * cos
        );
    }

    private void OnWeaponEquip(WeaponEquipEvent @event)
    {
        isCrossHairActive = true;
        crossHairImg.enabled = true;
        crossHairImg.sprite = @event.GunData.CrossHairSprite;

        recoilOffset = Vector2.zero;
        fireRecoilTimer = 0f;

        if (Mouse.current != null)
        {
            mousePos = Mouse.current.position.ReadValue();
            crossHairImg.rectTransform.position = mousePos;
            CHMousePos = mousePos;
        }
    }

    private void OnWeaponDrop(WeaponDropEvent @event)
    {
        isCrossHairActive = false;
        crossHairImg.enabled = false;

        recoilOffset = Vector2.zero;
        fireRecoilTimer = 0f;
    }
}