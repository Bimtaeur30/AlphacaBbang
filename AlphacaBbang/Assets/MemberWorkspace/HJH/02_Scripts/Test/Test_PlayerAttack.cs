using UnityEngine;
using UnityEngine.InputSystem;

public enum CharaterState
{
    None,
    Player,
    Enemy
}
public class Test_PlayerAttack : MonoBehaviour, IModule, IWeapon
{
    [SerializeField] private MeleeWeaponBase weapon;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private CharaterState charaterState;

    private void Start()
    {
        weapon.Init();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        switch (charaterState)
        {
            case CharaterState.None:
                Debug.Log($"상태가 None이라서 바꿔줘야함.{gameObject.name}");
                break;
            case CharaterState.Player:
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Vector3 targetPos = GetMouseWorldPoint();
                    weapon.Attack(targetPos, true);
                };
                break;
            case CharaterState.Enemy:
                break;
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

    public void Initialize(ModuleOwner owner)
    {

    }

    public void Init()
    {
        weapon.Init();
        weapon.charaterState = charaterState;
    }

    public void SetAim(bool val)
    {
        weapon.SetAim(val);
    }

    public void Attack(Vector3 vector, bool val)
    {
        weapon.Attack(vector, val);
    }
}