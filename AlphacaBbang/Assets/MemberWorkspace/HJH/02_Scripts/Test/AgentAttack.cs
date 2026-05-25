using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterState
{
    None,
    Player,
    Enemy
}

public class AgentAttack : MonoBehaviour, IModule, IEnemyWeaponModule, ICharacterStateOwner
{
    [SerializeField] private MeleeWeaponBase weapon;
    [SerializeField] private WeaponHolder weaponHolder;
    [SerializeField] private CharacterState characterState;
    public CharacterState CharacterState => characterState;

    private Camera _mainCamera;

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        if (weaponHolder != null)
            weaponHolder.OnMeleeWeaponChanged += SetCurrentWeapon;
    }

    private void OnDisable()
    {
        if (weaponHolder != null)
            weaponHolder.OnMeleeWeaponChanged -= SetCurrentWeapon;
    }

    public void SetCurrentWeapon(MeleeWeaponBase meleeWeapon)
    {
        if (weapon != null) weapon.gameObject.SetActive(false);
        weapon = meleeWeapon;
        if (weapon != null)
        {
            weapon.gameObject.SetActive(true);
            weapon.characterState = characterState;
        }
    }

    private void Update()
    {
        //switch (characterState)
        //{
        //    case CharacterState.None:
        //        Debug.Log($"상태가 None이라서 바꿔줘야함.{gameObject.name}");
        //        break;
        //    case CharacterState.Player:
        //        if (weapon != null && Mouse.current.leftButton.wasPressedThisFrame)
        //        {
        //            Vector3 targetPos = GetMouseWorldPoint();
        //            weapon.Attack(targetPos, true);
        //        }
        //        break;
        //    case CharacterState.Enemy:
        //        break;
        //}
    }

    private Vector3 GetMouseWorldPoint()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);
        return transform.position + transform.forward;
    }

    public void Initialize(ModuleOwner owner) { }

    public void Init()
    {
        if (weapon != null)
        {
            weapon.characterState = characterState;
        }
    }

    public void SetAim(bool val)
    {
        weapon?.SetAim(val);
    }

    public void Attack(Vector3 vector, bool val)
    {
        //weapon?.Attack(vector, val);
    }
}