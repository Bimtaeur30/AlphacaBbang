using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using Unity.Cinemachine;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CrossHairModule : MonoBehaviour, IModule
{
    public Vector2 CHMousePos { get; private set; }

    [SerializeField] private Image crossHairImg;
    [SerializeField] private EventChannelSO systemChannel;

    [Header("CrossHair Settings")]
    [SerializeField] private float defualtFollowSpeed = 20f;
    [SerializeField] private float recoilDistance = 60f;
    [SerializeField] private float recoilRecoverSpeed = 10f;

    private bool _isCrossHairActive;
    private Player_TJ _player;

    private Vector2 _mousePos;
    private Vector2 _recoilOffset;

    private CinemachineImpulseSource _impulseSource;

    // 연사 중 FireInterval마다 반동 적용
    private float _fireRecoilTimer;

    public void Initialize(ModuleOwner owner)
    {
        _player = owner as Player_TJ;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        Debug.Assert( _impulseSource != null ,"Impulse Source Componenet is NULL");

        Debug.Assert(_player != null, "CrossHairModule : player is null");
        Debug.Assert(crossHairImg != null, "CrossHairModule : crossHairImg is null");
        Debug.Assert(systemChannel != null, "CrossHairModule : systemChannel is null");
    }

    private void OnEnable()
    {
        systemChannel.AddListener<WeaponEquipEvent>(OnWeaponEquip);
        systemChannel.AddListener<WeaponDropEvent>(OnWeaponDrop);
    }

    private void OnDisable()
    {
        systemChannel.RemoveListener<WeaponEquipEvent>(OnWeaponEquip);
        systemChannel.RemoveListener<WeaponDropEvent>(OnWeaponDrop);
    }

    private void Update()
    {
        if (!_isCrossHairActive || crossHairImg == null || Mouse.current == null || _player == null)
            return;

        if (_player.GunHandleModule == null || _player.GunHandleModule.CurrentGun == null)
            return;

        _mousePos = Mouse.current.position.ReadValue();

        GunHandleModule gunHandle = _player.GunHandleModule;
        Gun currentGun = gunHandle.CurrentGun;
        GunDataSO gunData = currentGun.GunDataSO;

        bool isFiring = gunHandle.CurrentGun.IsFiring && gunHandle.CurrentGun.IsAiming && gunHandle.CurrentGun.Magazine.IsReloading == false;

        if (isFiring)
        {
            HandleFireRecoil(gunData);
        }
        else
        {
            // 발사 멈추면 다음 발사 때 즉시 반동 적용 가능하도록 초기화
            _fireRecoilTimer = 0f;
        }

        // 반동 오프셋 복구
        _recoilOffset = Vector2.Lerp(_recoilOffset, Vector2.zero, Time.deltaTime * recoilRecoverSpeed);

        Vector2 targetScreenPos = _mousePos + _recoilOffset;

        crossHairImg.rectTransform.position = Vector2.Lerp(
            crossHairImg.rectTransform.position,
            targetScreenPos,
            Time.deltaTime * defualtFollowSpeed
        );

        CHMousePos = crossHairImg.rectTransform.position;
    }

    private void HandleFireRecoil(GunDataSO gunData)
    {
        if (gunData == null)
            return;

        switch (gunData.FireMode)
        {
            case FireMode.Auto:
                HandleAutoFireRecoil(gunData);
                break;

            case FireMode.Single:
            case FireMode.Spread:
                HandleSingleLikeFireRecoil(gunData);
                break;
        }
    }

    private void HandleAutoFireRecoil(GunDataSO gunData)
    {
        float fireInterval = gunData.FireInterval;

        if (fireInterval <= 0f)
        {
            ApplyRecoil(gunData);
            return;
        }

        if (_fireRecoilTimer <= 0f)
        {
            ApplyRecoil(gunData);
            _fireRecoilTimer = fireInterval;
        }

        _fireRecoilTimer -= Time.deltaTime;
    }

    private void HandleSingleLikeFireRecoil(GunDataSO gunData)
    {
        // 누르고 있는 동안 한 번만 반동 적용
        if (_fireRecoilTimer > 0f)
            return;

        ApplyRecoil(gunData);
        _fireRecoilTimer = float.MaxValue;
    }

    private void ApplyRecoil(GunDataSO gunData)
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector2 playerScreenPos = cam.WorldToScreenPoint(_player.transform.position);

        Vector2 aimDir = _mousePos - playerScreenPos;
        if (aimDir.sqrMagnitude < 0.0001f)
            aimDir = Vector2.up;
        else
            aimDir.Normalize();

        float spreadAngle = gunData.SpreadAngle;
        float randomAngle = Random.Range(-spreadAngle, spreadAngle);

        Vector2 recoilDir = RotateVector(aimDir, randomAngle);
        _recoilOffset += recoilDir * Mathf.Sqrt(spreadAngle) * 60;

        crossHairImg.rectTransform.DOShakeRotation(0.1f, new Vector3(0, 0, gunData.RecoilForceX), 10, 90);

        PlayKick();
        ImpulseShake(aimDir, spreadAngle);
    }

    private void ImpulseShake(Vector2 dir, float force)
    {
        float adjusted = Mathf.Sqrt(force);
        Vector2 forceVector = dir * adjusted * 0.2f;

        _impulseSource.DefaultVelocity = forceVector;
        _impulseSource.GenerateImpulse();
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
        _isCrossHairActive = true;
        crossHairImg.enabled = true;
        crossHairImg.sprite = @event.GunData.CrossHairSprite;

        _recoilOffset = Vector2.zero;
        _fireRecoilTimer = 0f;

        if (Mouse.current != null)
        {
            _mousePos = Mouse.current.position.ReadValue();
            crossHairImg.rectTransform.position = _mousePos;
            CHMousePos = _mousePos;
        }
    }

    private void OnWeaponDrop(WeaponDropEvent @event)
    {
        _isCrossHairActive = false;
        crossHairImg.enabled = false;

        _recoilOffset = Vector2.zero;
        _fireRecoilTimer = 0f;
    }

    private void PlayKick()
    {
        float randShake = (Random.Range(-45f, 45f));
        crossHairImg.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(
            crossHairImg.rectTransform.DORotate(new Vector3(0, 0, randShake), 0.1f)
                  .SetEase(Ease.OutQuint)
        );

        seq.Join(
            crossHairImg.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f)
                   .SetEase(Ease.InSine)
        );

        seq.Append(
            crossHairImg.rectTransform.DORotate(Vector3.zero, 0.1f)
                  .SetEase(Ease.OutQuint)
        );

        seq.Join(
            crossHairImg.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f)
                .SetEase(Ease.InSine)
        );
    }
}