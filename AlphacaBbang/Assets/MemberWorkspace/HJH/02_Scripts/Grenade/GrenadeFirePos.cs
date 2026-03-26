using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeFirePos : MonoBehaviour
{
    [Header("발사 설정")]
    [SerializeField] private float firingAngle = 45.0f;
    [SerializeField] private float gravity = 9.8f;

    [Header("위치")]
    public Transform startPoint;
    private Transform targetPoint;

    [Header("프리팹")]
    public GameObject targetMark;
    public LayerMask layermask;

    [Header("무기 관리")]
    public List<GrenadeSO> grenadeList;
    public GrenadeSO currentGrenade;

    private int currentIndex = 0;

    private void Start()
    {
        layermask = 1 << LayerMask.NameToLayer("Floor");

        if (grenadeList.Count > 0)
            currentGrenade = grenadeList[0];
    }

    void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            MousePosition();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartCoroutine(SimulateProjectile());
            }
        }
    }

    public void MousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
        {
            if (targetPoint == null)
            {
                GameObject obj = Instantiate(targetMark, hit.point, Quaternion.identity);
                targetPoint = obj.transform;
            }
            else
            {
                targetPoint.position = hit.point;
            }
        }
    }

    public IEnumerator SimulateProjectile()
    {
        if (currentGrenade == null || currentGrenade.count <= 0)
        {
            Debug.Log("사용할 수 있는 폭탄 없음");
            yield break;
        }

        GameObject projectile = Instantiate(currentGrenade.prefab, startPoint.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        currentGrenade.count--;

        // 다 쓰면 다음 무기
        if (currentGrenade.count <= 0)
        {
            ChangeNextGrenade();
        }

        float distance = Vector3.Distance(startPoint.position, targetPoint.position);

        float angleRad = firingAngle * Mathf.Deg2Rad;
        float sinValue = Mathf.Sin(2 * angleRad);

        if (Mathf.Abs(sinValue) < 0.01f) yield break;

        float velocity = distance * gravity / sinValue;

        float Vx = Mathf.Sqrt(velocity) * Mathf.Cos(angleRad);
        float Vy = Mathf.Sqrt(velocity) * Mathf.Sin(angleRad);

        Vector3 direction = (targetPoint.position - startPoint.position).normalized;

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
        Debug.Log("모든 폭탄 소진");
    }
}