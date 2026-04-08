using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Agent
{
    [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }

    [SerializeField] private float _rotationSpeed = 0;

    private IControllerMovement _movement;
    private PlayerStaminaGaugeSystem _stamina;

    private PlayerStatSystem _stat;

    private Vector2 _lookInput;
    public bool IsAiming {get; private set;}

    protected override void Awake()
    {
        base.Awake();

        _movement = GetModule<IControllerMovement>();
        _stamina = GetComponentInChildren<PlayerStaminaGaugeSystem>();
        _stat = GetComponentInChildren<PlayerStatSystem>();

        PlayerInput.OnMovementChange += HandleMovement;
        PlayerInput.OnLookChange += HandleLook;
        PlayerInput.OnAim += HandleAim;
        PlayerInput.OnSprint += HandleSprint;
    }
    private void Update()
    {
        if (IsAiming)
            RotateToMouse();
        else
            RotateToMovement();
    }
    private void RotateToMovement()
    {
        var movement = _movement as AgentMovement;
        Vector3 velocity = movement.Velocity;

        velocity.y = 0;

        if (velocity.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(velocity);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            8f * Time.deltaTime);
    }
    private void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y;

            Vector3 dir = (lookPoint - transform.position).normalized;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleMovement(Vector2 input)
    {
        _movement.SetMovementDirection(input);
    }

    private void HandleLook(Vector2 input)
    {
        _lookInput = input;
    }

    private void HandleAim(bool isAiming)
    {
        if (_stamina != null)
        {
            if(!_stamina.CanAim)
            {
                isAiming = false;
            }
        }

        IsAiming = isAiming;

        var movement = _movement as AgentMovement;
        movement.SetSpeedMultiplier(isAiming ? 0.3f : 1f);
        movement.SetUseRotation(!isAiming);
    }

    private void HandleSprint(bool isSprinting)
    {
        if (_stat != null && !_stat.CanRun())
            isSprinting = false;

        _stat.SetRunning(isSprinting);

        var movement = _movement as AgentMovement;

        if (movement != null)
        {
            movement.SetSpeedMultiplier(isSprinting ? 1.5f : 1f);
        }
    }

    private void OnDestroy()
    {
        PlayerInput.OnMovementChange -= HandleMovement;
        PlayerInput.OnLookChange -= HandleLook;
        PlayerInput.OnAim -= HandleAim;
    }
}