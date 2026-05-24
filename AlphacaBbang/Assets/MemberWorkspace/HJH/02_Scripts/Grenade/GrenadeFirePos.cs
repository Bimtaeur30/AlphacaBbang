using Assets.MemberWorkspace.HJH._02_Scripts.Grenade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeFirePos : MonoBehaviour, IModule, IWeapon, ICharacterStateOwner
{
    [SerializeField] private CharacterState characterState;
    public CharacterState CharacterState => characterState;

    [Header("발사 설정")]
    [SerializeField] private float firingAngle = 45.0f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float throwingDistance = 5.0f;

    [Header("위치")]
    public Transform startPoint;
    private Transform targetPoint;
    public bool HasTarget => targetPoint != null;
    public Vector3 TargetWorldPos => _targetWorldPos;

    [Header("프리팹")]
    public GameObject targetMark;
    public LayerMask layermask;

    [Header("무기 관리")]
    public List<GrenadeSO> grenadeList;
    public GrenadeSO currentGrenade;

    [Header("포물선 라인렌더러")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int lineSegmentCount = 30;
    [SerializeField] private float lineTimeStep = 0.1f;

    private int currentIndex = 0;
    private Vector3 _targetWorldPos;
    private bool _isAiming;
    public GunDataSO WeaponData => null;
    public bool IsFiring => false;
    public bool IsAiming => _isAiming;
    public bool IsReloading => false;
    public void Initialize(WeaponHandleModule owner) { }
    public void TickFire() { }
    public void StopFire(bool isAim) { }

    public bool IsReady => currentGrenade != null && currentGrenade.count > 0;

    public event System.Action OnFired;

    private void Start()
    {
        if (grenadeList.Count > 0)
            currentGrenade = grenadeList[0];
    }

    private void Update()
    {
        if (characterState != CharacterState.Player) return;
        if (!_isAiming) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.isPressed)
        {
            Vector3? targetPos = GetMouseWorldPosition();
            if (targetPos.HasValue)
                SetTarget(targetPos.Value);

            if (mouse.leftButton.wasPressedThisFrame)
            {
                StartFire(true);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
            ClearTargetPoint();
        }
    }
    public void SetAim(bool isAim)
    {
        //Debug.Log($"@@@@@@@@@@@@@@@@@@@@@@@@@[GrenadeFirePos] SetAim 호출됨: {isAim}");
        _isAiming = isAim;
        if (!isAim)
        {
            lineRenderer.positionCount = 0;
            ClearTargetPoint();
        }
    }
    public void StartFire(bool isAim)
    {
        if (!IsReady)
        {
            Debug.LogWarning("[GrenadeFirePos] 사용할 수 있는 수류탄 없음");
            SetAim(false);
            return;
        }

        StartCoroutine(SimulateProjectile(_targetWorldPos, true, currentGrenade));
        OnFired?.Invoke();

        if (!IsReady)
            SetAim(false);
    }
    public void SetCurrentGrenade(GrenadeSO grenade)
    {
        currentGrenade = grenade;
    }

    public void SetTarget(Vector3 worldPosition)
    {
        Vector3 dir = worldPosition - startPoint.position;
        if (dir.magnitude > throwingDistance)
            dir = dir.normalized * throwingDistance;

        _targetWorldPos = startPoint.position + dir;

        if (targetPoint == null)
        {
            GameObject obj = Instantiate(targetMark, _targetWorldPos, Quaternion.identity);
            targetPoint = obj.transform;
        }
        else
        {
            targetPoint.position = _targetWorldPos;
        }

        DrawTrajectory(_targetWorldPos);
    }

    public void Attack(Vector3 vector, bool val)
    {
        StartCoroutine(SimulateProjectile(vector, val, currentGrenade));
    }

    public IEnumerator SimulateProjectile(Vector3 targetPos, bool val, GrenadeSO grenade)
    {
        if (grenade == null || grenade.count <= 0)
        {
            Debug.Log("사용할 수 있는 폭탄 없음");
            yield break;
        }

        if (!val)
        {
            Debug.Log("공격 못함");
            yield break;
        }

        GameObject projectile = Instantiate(grenade.prefab, startPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        grenade.count--;
        if (grenade.count <= 0)
            ChangeNextGrenade();

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

    private Vector3? GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layermask))
            return hit.point;
        return null;
    }

    private void ClearTargetPoint()
    {
        if (targetPoint != null)
        {
            Destroy(targetPoint.gameObject);
            targetPoint = null;
        }
    }

    private void ChangeNextGrenade()
    {
        for (int i = 0; i < grenadeList.Count; i++)
        {
            currentIndex = (currentIndex + 1) % grenadeList.Count;
            if (grenadeList[currentIndex].count > 0)
            {
                currentGrenade = grenadeList[currentIndex];
                return;
            }
        }
        currentGrenade = null;
    }

    public void Initialize(ModuleOwner owner) { }
    public void Init() { }

    private void OnDrawGizmos()
    {
        if (startPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint.position, throwingDistance);
    }
}