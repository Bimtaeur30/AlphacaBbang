using JJH._02_Scripts.Weapons;
using System.Collections;
using UnityEngine;

public abstract class GrenadeBehavior : WeaponBase, IWeapon
{
    [Header("발사 설정")]
    [SerializeField] private float firingAngle = 45.0f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float throwingDistance = 5.0f;

    [Header("위치")]
    [SerializeField] public Transform startPoint;
    [SerializeField] public LayerMask layermask;

    [Header("타겟 마크")]
    [SerializeField] private GameObject targetMarkPrefab;
    private Transform targetPoint;
    private Vector3 _targetWorldPos;

    [Header("포물선 라인렌더러")]
    [SerializeField] public LineRenderer lineRenderer;
    [SerializeField] private int lineSegmentCount = 30;
    [SerializeField] private float lineTimeStep = 0.1f;

    private GrenadeSO _grenadeSO;

    public GunDataSO WeaponData => null;
    public bool IsFiring => false;
    public bool IsAiming => _isAiming;
    public bool IsReloading => false;
    public void TickFire() { }
    public void StopFire(bool isAim) { }

    public event System.Action OnFired;

    private bool _isAiming;
    public bool HasTarget => targetPoint != null;
    public Vector3 TargetWorldPos => _targetWorldPos;

    protected virtual void Awake()
    {
        if (startPoint == null)
            startPoint = transform;
    }

    public virtual void Initialize(WeaponHandleModule owner) { }

    public void Setup(GrenadeSO grenadeSO)
    {
        _grenadeSO = grenadeSO;
    }

    public void SetAim(bool isAim)
    {
        _isAiming = isAim;
        if (!isAim)
        {
            if (lineRenderer != null)
                lineRenderer.positionCount = 0;
            ClearTargetPoint();
        }
    }

    public void StartFire(bool isAim)
    {
        if (_grenadeSO == null)
        {
            Debug.LogWarning("[GrenadeBehavior] grenadeSO가 null입니다.");
            return;
        }

        Vector3 spawnPos = startPoint.position;
        Vector3 targetPos = _targetWorldPos;

        ClearTargetPoint();
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;

        OnFired?.Invoke();

        GrenadeProjectileLauncher.Launch(_grenadeSO, spawnPos, targetPos, firingAngle, gravity, this);

        Destroy(gameObject);
    }

    public void SetTarget(Vector3 worldPosition)
    {
        Vector3 dir = worldPosition - startPoint.position;
        if (dir.magnitude > throwingDistance)
            dir = dir.normalized * throwingDistance;

        _targetWorldPos = startPoint.position + dir;

        DrawTrajectory(_targetWorldPos);
    }

    private void Update()
    {
        if (_isAiming)
            DrawTrajectory(_targetWorldPos);
        else if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }

    private void DrawTrajectory(Vector3 targetPos)
    {
        if (lineRenderer == null) return;

        lineRenderer.useWorldSpace = true;

        Vector3 direction = (targetPos - startPoint.position);
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance < 0.01f) return;

        direction = direction.normalized;

        float angleRad = firingAngle * Mathf.Deg2Rad;
        float sinValue = Mathf.Sin(2 * angleRad);
        if (Mathf.Abs(sinValue) < 0.01f) return;

        float velocity = distance * gravity / sinValue;
        float Vx = Mathf.Sqrt(velocity) * Mathf.Cos(angleRad);
        float Vy = Mathf.Sqrt(velocity) * Mathf.Sin(angleRad);

        Vector3 velocityVector = new Vector3(direction.x * Vx, Vy, direction.z * Vx);

        float totalTime = distance / Vx;
        float timeStep = totalTime / lineSegmentCount;

        lineRenderer.positionCount = lineSegmentCount;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPoint.position
                            + velocityVector * t
                            + Vector3.down * (0.5f * gravity * t * t);
            lineRenderer.SetPosition(i, point);
        }
    }

    private void ClearTargetPoint()
    {
        if (targetPoint != null)
        {
            Destroy(targetPoint.gameObject);
            targetPoint = null;
        }
    }

    [Header("Blink Settings")]
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private float blinkStartTime = 1f;
    [SerializeField] private float maxBlinkSpeed = 10f;
    private Material originalMaterial;
    private Renderer grenadeRenderer;

    public IEnumerator Boom(GameObject projectile, float boomTime)
    {
        grenadeRenderer = projectile.GetComponent<Renderer>();
        if (grenadeRenderer != null)
            originalMaterial = grenadeRenderer.material;

        float waitTime = boomTime - blinkStartTime;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(BlinkRoutine(blinkStartTime));
        OnExplode();
        Destroy(projectile);
    }

    private IEnumerator BlinkRoutine(float duration)
    {
        float timer = 0f;
        bool isBlink = false;
        while (timer < duration)
        {
            float t = timer / duration;
            float blinkSpeed = Mathf.Lerp(1f, maxBlinkSpeed, t);
            float interval = 1f / blinkSpeed;
            isBlink = !isBlink;
            grenadeRenderer.material = isBlink ? blinkMaterial : originalMaterial;
            timer += interval;
            yield return new WaitForSeconds(interval);
        }
        grenadeRenderer.material = originalMaterial;
    }

    protected abstract void OnExplode();

    private void OnDrawGizmos()
    {
        if (startPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint.position, throwingDistance);
    }
}