using UnityEngine;
using UnityEngine.InputSystem;

public class Test_EnemyAttack : MonoBehaviour
{
    [SerializeField] private MeleeWeaponBase weapon;
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        weapon.Init();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 targetPos = GetMouseWorldPoint();
            weapon.Attack(targetPos, true);
        }
    }

    private Vector3 GetMouseWorldPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position + transform.forward;
    }
}
