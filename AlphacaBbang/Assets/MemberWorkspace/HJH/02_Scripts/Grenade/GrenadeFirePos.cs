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

    private void Start()
    {
        //if(layermask == 0)
        //{
        //    layermask = 1 << LayerMask.NameToLayer("Obstacle");
        //}

        if (grenadeList.Count > 0)
            currentGrenade = grenadeList[0];
    }

    void Update()
    {
        switch (characterState)
        {
            case CharacterState.None:
                Debug.Log($"상태가 None이라서 바꿔줘야함.{gameObject.name}");
                break;
            case CharacterState.Player:
                if (Mouse.current.rightButton.isPressed)
                {
                    MousePosition();
                    Vector3 direction = targetMark.transform.position;
                    DrawTrajectory(direction);

                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        bool canAttack = true; // 임시로 박아둠
                        StartCoroutine(SimulateProjectile(direction, canAttack, currentGrenade));
                    }
                }
                else
                {
                    lineRenderer.positionCount = 0;
                    ClearTargetPoint();
                }
                break;
            case CharacterState.Enemy:
                break;
        }
    }

    public void MousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
        {
            Vector3 dir = hit.point - startPoint.position;
            if (dir.magnitude > throwingDistance)
                dir = dir.normalized * throwingDistance;

            Vector3 finalPos = startPoint.position + dir;
            if (targetPoint == null)
            {
                GameObject obj = Instantiate(targetMark, finalPos, Quaternion.identity);
                targetPoint = obj.transform;
            }
            else
            {
                targetPoint.position = finalPos;
            }
        }
    }
    public void ClearTargetPoint()
    {
        if (targetPoint != null)
        {
            Destroy(targetPoint.gameObject);
            targetPoint = null;
        }
    }

    public IEnumerator SimulateProjectile(Vector3 direction, bool val, GrenadeSO currentGrenade)
    {
        if (currentGrenade == null || currentGrenade.count <= 0)
        {
            Debug.Log("사용할 수 있는 폭탄 없음");
            yield break;
        }
        else if (!val)
        {
            Debug.Log("공격 못함");
            yield break;
        }

        GameObject projectile = Instantiate(currentGrenade.prefab, startPoint.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        currentGrenade.count--;

        if (currentGrenade.count <= 0)
        {
            ChangeNextGrenade();
        }

        direction = (direction - startPoint.position).normalized;

        float distance = Vector3.Distance(startPoint.position, direction);

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

    void ChangeNextGrenade()
    {
        for (int i = 0; i < grenadeList.Count; i++)
        {
            currentIndex = (currentIndex + 1) % grenadeList.Count;

            if (grenadeList[currentIndex].count > 0)
            {
                currentGrenade = grenadeList[currentIndex];
                Debug.Log("다음 무기로 변경: " + currentGrenade.grenadeName);
                return;
            }
        }

        currentGrenade = null;
        Debug.Log("폭탄 없어");
    }
    void OnDrawGizmos()
    {
        if (startPoint == null) return;

        Gizmos.color = Color.green;

        float range = throwingDistance;

        Gizmos.DrawWireSphere(startPoint.position, range);
    }

    void DrawTrajectory(Vector3 direction)
    {
        if (targetPoint == null) return;
        direction = (direction - startPoint.position).normalized;

        float distance = Vector3.Distance(startPoint.position, direction);

        float angleRad = firingAngle * Mathf.Deg2Rad;
        float sinValue = Mathf.Sin(2 * angleRad);

        if (Mathf.Abs(sinValue) < 0.01f) return;

        float velocity = distance * gravity / sinValue;

        float Vx = Mathf.Sqrt(velocity) * Mathf.Cos(angleRad);
        float Vy = Mathf.Sqrt(velocity) * Mathf.Sin(angleRad);

        Vector3 velocityVector = new Vector3(direction.x * Vx, Vy - 0.5f, direction.z * Vx);

        lineRenderer.positionCount = lineSegmentCount;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float t = i * lineTimeStep;

            Vector3 point = startPoint.position +
                            velocityVector * t +
                            0.5f * Physics.gravity * t * t;

            lineRenderer.SetPosition(i, point);
        }
    }

    public void Initialize(ModuleOwner owner)
    {

    }

    public void Init()
    {

    }

    public void SetAim(bool val)
    {
        // 애니메이션 추가 예정
    }

    public void Attack(Vector3 vector, bool val)
    {
        StartCoroutine(SimulateProjectile(vector, val, currentGrenade));
    }
}