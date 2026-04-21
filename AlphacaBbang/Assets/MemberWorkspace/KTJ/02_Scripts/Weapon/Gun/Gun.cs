using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts_Systems.AnimationSystems;
using System;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunRenderer Renderer { get; private set; }
    public bool IsAiming { get; private set; }
    public bool IsFiring { get; private set; }

    [field: SerializeField] public GunDataSO GunDataSO { get; private set; }
    [field: SerializeField] public LayerMask TargetLayer { get; private set; }
    [field: SerializeField] public Magazine Magazine { get; private set; }

    [Header("Fire")]
    [SerializeField] protected Transform firePos;
    [SerializeField] protected float rayDistance = 10f;
    [SerializeField] protected float damage = 10f;
    //[SerializeField] protected LineRenderer lineRenderer;

    [Header("Anim")]
    [SerializeField] private AnimParamSO idleAnimParam;
    [SerializeField] private AnimParamSO aimAnimParam;
    [SerializeField] private AnimParamSO singleFireAnimParam;
    [SerializeField] private AnimParamSO autoFireAnimParam;

    [Header("Pool")]
    [SerializeField] protected PoolManagerSO poolManager;
    [SerializeField] protected PoolItemSO lineRenderer;
    [SerializeField] protected PoolItemSO bulletParticle;

    protected float _lastFireTime = -999f;

    protected GunHandleModule _gunHandleModule;

    protected virtual void Awake()
    {
        Renderer = GetComponentInChildren<GunRenderer>();


        Debug.Assert(Renderer != null, "GunRenderer가 자식으로 붙어있지 않습니다.");
        Debug.Assert(firePos != null, "firePos가 할당되지 않았습니다.");
    }

    public virtual void Initialize(GunHandleModule module)
    {
        IsAiming = false;
        IsFiring = false;
        _gunHandleModule = module;
        Debug.Assert(_gunHandleModule != null, "건핸들러모듈을 받아오지 못했습니다.");

        Magazine = GetComponentInChildren<Magazine>();
        Magazine.Initialize(this);
        Debug.Assert(Magazine != null, "Magazine.cs가 자식으로 붙어있지 않습니다.");
    }

    public virtual void SetAim(bool isAim)
    {
        IsAiming = isAim;

        if (IsFiring && GunDataSO.FireMode == FireMode.Auto && IsAiming)
        {
            PlayAutoFire();
            return;
        }

        if (IsAiming)
            PlayAim();
        else
            PlayIdle();
    }

    public virtual void StartFire(bool isAim)
    {
        IsFiring = true;

        if (!isAim)
            return;

        switch (GunDataSO.FireMode)
        {
            case FireMode.Single:
            case FireMode.Spread:
                PlaySingleFire();
                TryFire();
                break;

            case FireMode.Auto:
                PlayAutoFire();
                TryFire();
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
        if (!IsAiming || !IsFiring)
            return;

        if (GunDataSO.FireMode == FireMode.Auto)
        {
            TryFire();
        }
    }

    public void TryFire()
    {
        if (!CanFire())
            return;

        _lastFireTime = Time.time;
        if (Magazine.TryUseBullet())
        {
            FireInternal();
        }
        else
        {
            Action onReloadEnd = new Action(OnReloadEnd);
            Magazine.TryReload(onReloadEnd);
            StopFire(IsAiming);
        }
    }

    private void OnReloadEnd()
    {
        //StartFire(_isFiring && _isAiming);
    }

    protected virtual bool CanFire()
    {
        return Time.time >= _lastFireTime + GunDataSO.FireInterval;
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
        if (aimAnimParam != null && Renderer != null)
            Renderer.PlayClip(aimAnimParam.ParamHash, 0, 0.1f, 0);
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
            float animSpeed = 0.083f / GunDataSO.FireInterval;
            Renderer.PlayClip(autoFireAnimParam.ParamHash, 0, 0.1f, 0, animSpeed);
        }
    }

    protected abstract void OnFire(Vector3 origin, Vector3 direction);
    protected abstract void OnHit(RaycastHit hit);
    protected abstract void OnMiss(Vector3 origin, Vector3 endPoint);
}