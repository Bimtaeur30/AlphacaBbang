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

    [Header("수류탄")]
    [SerializeField] protected GrenadeSO grenadeSO;

    // IWeapon 구현
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
        if (grenadeSO == null)
        {
            Debug.LogWarning("[GrenadeBehavior] grenadeSO가 null입니다.");
            return;
        }

        StartCoroutine(FireAndDestroy(_targetWorldPos, grenadeSO));
        OnFired?.Invoke();
    }

    private IEnumerator FireAndDestroy(Vector3 targetPos, GrenadeSO grenade)
    {
        yield return StartCoroutine(SimulateProjectile(targetPos, grenade));
        Destroy(gameObject);
    }

    public void SetTarget(Vector3 worldPosition)
    {
        Vector3 dir = worldPosition - startPoint.position;
        if (dir.magnitude > throwingDistance)
            dir = dir.normalized * throwingDistance;

        _targetWorldPos = startPoint.position + dir;

        if (targetPoint == null)
        {
            GameObject obj = Instantiate(targetMarkPrefab, _targetWorldPos, Quaternion.identity);
            targetPoint = obj.transform;
        }
        else
        {
            targetPoint.position = _targetWorldPos;
        }

        DrawTrajectory(_targetWorldPos);
    }

    private IEnumerator SimulateProjectile(Vector3 targetPos, GrenadeSO grenade)
    {
        GameObject projectile = Instantiate(grenade.prefab, startPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 direction = (targetPos - startPoint.position).normalized;
        float distance = Vector3.Distance(startPoint.position, targetPos);

        float angleRad = firingAngle * Mathf.Deg2Rad;
        float sinValue = Mathf.Sin(2 * angleRad);
        if (Mathf.Abs(sinValue) < 0.01f) yield break;

        float velocity = distance * gravity / sinValue;
        float Vx = Mathf.Sqrt(velocity) * Mathf.Cos(angleRad);
        float Vy = Mathf.Sqrt(velocity) * Mathf.Sin(angleRad);

        projectile.transform.rotation = Quaternion.LookRotation(direction);
        rb.linearVelocity = new Vector3(direction.x * Vx, Vy, direction.z * Vx);

        yield return null;
    }

    private void DrawTrajectory(Vector3 targetPos)
    {
        if (targetPoint == null) return;

        Vector3 direction = (targetPos - startPoint.position).normalized;
        float distance = Vector3.Distance(startPoint.position, targetPos);

        float angleRad = firingAngle * Mathf.Deg2Rad;
        float sinValue = Mathf.Sin(2 * angleRad);
        if (Mathf.Abs(sinValue) < 0.01f) return;

        float velocity = distance * gravity / sinValue;
        float Vx = Mathf.Sqrt(velocity) * Mathf.Cos(angleRad);
        float Vy = Mathf.Sqrt(velocity) * Mathf.Sin(angleRad);

        Vector3 velocityVector = new Vector3(direction.x * Vx, Vy, direction.z * Vx);
        lineRenderer.positionCount = lineSegmentCount;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float t = i * lineTimeStep;
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