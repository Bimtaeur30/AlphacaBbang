using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Weapons;
using JJH._02_Scripts_Systems.AnimationSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public abstract class Gun : WeaponBase, IWeapon
{
    public GunRenderer Renderer { get; private set; }
    public bool IsAiming { get; private set; }
    public bool IsFiring { get; private set; }
    public bool IsReloading { get; private set; }

    [field: SerializeField] public GunDataSO WeaponData { get; private set; }
    [field: SerializeField] public LayerMask TargetLayer { get; private set; }
    [field: SerializeField] public Magazine Magazine { get; private set; }

    [Header("Fire")]
    [SerializeField] protected Transform firePos;
    [SerializeField] protected float rayDistance = 10f;
    [SerializeField] protected float damage = 10f;

    [Header("Anim")]
    [SerializeField] private AnimParamSO singleFireAnimParam;
    [SerializeField] private AnimParamSO autoFireAnimParam;
    [SerializeField] private AnimParamSO idleAnimParam;

    [Header("Pool")]
    [SerializeField] protected PoolManagerSO poolManager;
    [SerializeField] protected PoolItemSO lineRenderer;
    [SerializeField] protected PoolItemSO bulletParticle;

    [Header("System")]
    [SerializeField] private EventChannelSO gunChannel;

    protected float _lastFireTime = -999f;
    protected WeaponHandleModule _gunHandleModule;
    protected GunSoundPlayer _gunSoundPlayuer;

    protected virtual void Awake()
    {
        Renderer = GetComponentInChildren<GunRenderer>();
        _gunSoundPlayuer = GetComponentInChildren<GunSoundPlayer>();
        Debug.Assert(Renderer != null, "GunRenderer가 자식으로 붙어있지 않습니다.");
        Debug.Assert(_gunSoundPlayuer != null, "GunSoundPlayer가 자식으로 붙어있지 않습니다.");

        Debug.Assert(firePos != null, "firePos가 할당되지 않았습니다.");
    }

    public virtual void Initialize(WeaponHandleModule module)
    {
        IsAiming = false;
        IsFiring = false;
        _gunHandleModule = module;
        Debug.Assert(_gunHandleModule != null, "건핸들러모듈을 받아오지 못했습니다.");

        Magazine = GetComponentInChildren<Magazine>();
        Magazine.Initialize(this);
        Debug.Log("탄창 초기화 완료");
        Debug.Assert(Magazine != null, "Magazine.cs가 자식으로 붙어있지 않습니다.");

        gunChannel.RaiseEvent(GunEvents.WeaponEquipDataEvent.Init(WeaponData));
    }

    public virtual void SetAim(bool isAim)
    {
        IsAiming = isAim;

        if (!IsAiming)
        {
            IsFiring = false;
            PlayIdle();
            return;
        }

        if (IsFiring && !Magazine.IsReloading && WeaponData.FireMode == FireMode.Auto)
        {
            PlayAutoFire();
            return;
        }

        PlayAim();
    }

    public virtual void StartFire(bool isAim)
    {
        IsFiring = true;

        if (!isAim || Magazine.IsReloading)
            return;

        switch (WeaponData.FireMode)
        {
            case FireMode.Single:
            case FireMode.Spread:
            case FireMode.Melee:
                if (TryFire())
                    PlaySingleFire();
                break;

            case FireMode.Auto:
                if (TryFire())
                    PlayAutoFire();
                break;
        }
    }

    public virtual void StopFire(bool isAim)
    {
        IsFiring = false;

        if (isAim)
            PlayAim();
        else
            PlayIdle();
    }

    public virtual void TickFire()
    {
        if (!IsAiming || !IsFiring || Magazine.IsReloading)
            return;

        if (WeaponData.FireMode == FireMode.Auto)
        {
            TryFire();
        }
    }

    public bool TryFire()
    {
        if (!CanFire())
        {
            //_gunSoundPlayuer.PlaySound(GunDataSO.DryFireClip);
            return false;
        }
        if (WeaponData.FireMode == FireMode.Melee)
        {
            return true;
        }

        //==========================================================//

        _lastFireTime = Time.time;
        if (Magazine.TryUseBullet())
        {
            FireInternal();
            _gunSoundPlayuer.PlaySound(WeaponData.FireClip);
            return true;
        }
        else
        {
            if (Magazine.TryReload(OnReloadEnd))
            {
                OnReloadStart();
            }// 람다/Action 래핑 없이 직접 전달
            StopFire(IsAiming);               // IsFiring = false
            return false;
        }
    }

    // 장전 시작 시 호출 (서브클래스 확장용)
    protected virtual void OnReloadStart()
    {
        _gunSoundPlayuer.PlaySound(WeaponData.UnLoadClip);
        IsReloading = true;
    }

    // 장전 완료 시 호출 — 기본은 아무것도 안 함 (플레이어는 다시 입력해야 발사)
    // 적처럼 자동 재개가 필요한 경우 GunHandleModule에 위임
    protected virtual void OnReloadEnd()
    {
        _gunHandleModule.OnReloadEnd();
        _gunSoundPlayuer.PlaySound(WeaponData.LoadClip);
        _gunSoundPlayuer.PlaySound(WeaponData.CookClip);
        IsReloading = false;
    }

    protected virtual bool CanFire()
    {
        return Time.time >= _lastFireTime + WeaponData.FireInterval;
    }

    protected virtual void FireInternal()
    {
        Vector3 origin = firePos.position;
        Vector3 direction = GetShootDirection();
        Vector3 endPoint = origin + direction * rayDistance;

        OnFire(origin, direction);

        Debug.DrawRay(origin, direction * rayDistance, Color.red, 0.2f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, TargetLayer))
        {
            endPoint = hit.point;

            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }

            OnHit(hit);
        }
        else
        {
            OnMiss(origin, endPoint);
        }

        if (lineRenderer != null)
        {
            DrawLine(origin, endPoint, 0.05f);
        }

        _gunHandleModule.OnFire();
    }

    protected virtual Vector3 GetShootDirection()
    {
        return transform.right.normalized;
    }

    protected void DrawLine(Vector3 origin, Vector3 endPoint, float time)
    {
        PoolLineRendererEffect effect = poolManager.Pop<PoolLineRendererEffect>(lineRenderer);
        effect.StartCoroutine(effect.DrawLineRenderer(origin, endPoint, time));
    }

    protected void PlayIdle()
    {
        if (idleAnimParam != null && Renderer != null)
            Renderer.PlayClip(idleAnimParam.ParamHash, 0, 0.1f, 0);
    }
    protected void PlayAim()
    {
        if (idleAnimParam != null && Renderer != null)
            Renderer.PlayClip(idleAnimParam.ParamHash, 0, 0.1f, 0);
    }

    protected virtual void PlaySingleFire()
    {
        if (singleFireAnimParam != null && Renderer != null)
            Renderer.PlayClip(singleFireAnimParam.ParamHash, 0, 0.1f, 0);
    }

    protected virtual void PlayAutoFire()
    {
        if (autoFireAnimParam != null && Renderer != null)
        {
            float animSpeed = 0.083f / WeaponData.FireInterval;
            Renderer.PlayClip(autoFireAnimParam.ParamHash, 0, 0.1f, 0, animSpeed);
        }
    }

    protected abstract void OnFire(Vector3 origin, Vector3 direction);
    protected abstract void OnHit(RaycastHit hit);
    protected abstract void OnMiss(Vector3 origin, Vector3 endPoint);
}