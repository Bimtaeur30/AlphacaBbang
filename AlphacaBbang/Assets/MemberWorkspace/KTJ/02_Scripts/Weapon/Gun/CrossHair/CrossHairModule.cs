using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CrossHairModule : MonoBehaviour, IModule
{
    public Vector2 CHMousePos { get; private set; }

    [SerializeField] private EventChannelSO gunChannel;

    [Header("CrossHair Settings")]
    [SerializeField] private float defualtFollowSpeed = 20f;
    [SerializeField] private float recoilDistance = 60f;
    [SerializeField] private float recoilRecoverSpeed = 10f;
    [SerializeField] private float impulseShakeForceMultiply = 0.2f;

    private Image crossHairImg;
    private bool _isCrossHairActive;
    private PlayerController _player;
    [Reflex.Attributes.Inject] private CursorController _cursorController;

    private Vector2 _mousePos;
    private Vector2 _recoilOffset;

    private CinemachineImpulseSource _impulseSource;

    // ���� �� FireInterval���� �ݵ� ����
    private float _fireRecoilTimer;

    public void Initialize(ModuleOwner owner)
    {
        _player = owner as PlayerController;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        crossHairImg = _cursorController.GetGunCursor();
        Debug.Assert( _impulseSource != null ,"Impulse Source Componenet is NULL");
        Debug.Assert(_cursorController != null ,"커서 컨트롤러를 가져오지 못했습니다.");
        Debug.Assert(crossHairImg != null ,"크로스 헤어 이미지를 정상적으로 가져오지 못했습니다. 커서 컨트롤러가 정상적으로 작동하는지 확인하세요.");

        Debug.Assert(_player != null, "CrossHairModule : player is null");
        Debug.Assert(crossHairImg != null, "CrossHairModule : crossHairImg is null");
        Debug.Assert(gunChannel != null, "CrossHairModule : systemChannel is null");
    }

    private void OnEnable()
    {
        gunChannel.AddListener<WeaponEquipDataEvent>(OnWeaponEquip);
        gunChannel.AddListener<WeaponDropEvent>(OnWeaponDrop);
    }

    private void OnDisable()
    {
        gunChannel.RemoveListener<WeaponEquipDataEvent>(OnWeaponEquip);
        gunChannel.RemoveListener<WeaponDropEvent>(OnWeaponDrop);
    }

    private void Update()
    {
        if (!_isCrossHairActive || crossHairImg == null || Mouse.current == null || _player == null)
            return;

        if (_player.GunHandleModule == null || _player.GunHandleModule.CurrentGun == null)
            return;

        _mousePos = Mouse.current.position.ReadValue();

        PlayerGunHandleModule gunHandle = _player.GunHandleModule;
        Gun currentGun = gunHandle.CurrentGun;
        GunDataSO gunData = currentGun.GunDataSO;

        bool isFiring = gunHandle.CurrentGun.IsFiring && gunHandle.CurrentGun.IsAiming && gunHandle.CurrentGun.Magazine.IsReloading == false;

        if (isFiring)
        {
            HandleFireRecoil(gunData);
        }
        else
        {
            // �߻� ���߸� ���� �߻� �� ��� �ݵ� ���� �����ϵ��� �ʱ�ȭ
            _fireRecoilTimer = 0f;
        }

        // �ݵ� ������ ����
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
        // ������ �ִ� ���� �� ���� �ݵ� ����
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
        Vector2 forceVector = dir * adjusted * impulseShakeForceMultiply;

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

    private void OnWeaponEquip(WeaponEquipDataEvent @event)
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