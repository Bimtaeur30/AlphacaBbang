using JJH._02_Scripts_Systems.AnimationSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Agent
{
    [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }

    [SerializeField] private AnimParamSO _isGunParam;
    [SerializeField] private AnimParamSO _speedParam;
    [SerializeField] private AnimParamSO _attackWalkParam;
    [SerializeField] private float _rotationSpeed = 10f;

    private IRenderer _renderer;
    private AgentMovement _agentMovement;

    private IControllerMovement _movement;
    private PlayerStaminaGaugeSystem _stamina;
    private PlayerStatSystem _stat;
    
    private PlayerEnumState _playerEnumState;

    public bool IsAiming { get; private set; }

    private Vector2 _movementInput;

    protected override void Awake()
    {
        base.Awake();

        _movement = GetModule<IControllerMovement>();
        _agentMovement = _movement as AgentMovement;
        _renderer = GetModule<IRenderer>();

        _stamina = GetComponentInChildren<PlayerStaminaGaugeSystem>();
        _stat = GetComponentInChildren<PlayerStatSystem>();

        PlayerInput.OnMovementChange += HandleMovement;
        PlayerInput.OnAimAction += HandleAim;
        PlayerInput.OnSprintAction += HandleSprint;
    }

    private void Update()
    {
        if (IsAiming)
            RotateToMouse();
        else
            RotateToMovement();

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        float speed = _agentMovement.Velocity.magnitude;
        float normalized = speed / 8f;
        if (normalized < 0.5f)
            normalized = 0f;

        _renderer.SetFloat(_speedParam.ParamHash, normalized, 0.1f, Time.deltaTime);

        _renderer.SetBool(_isGunParam.ParamHash, IsAiming);

        if (IsAiming)
        {
            float attackWalk = GetAttackWalkValue(_movementInput);

            _renderer.SetFloat(_attackWalkParam.ParamHash, attackWalk, 0.1f, Time.deltaTime);
        }
    }
    private float GetAttackWalkValue(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
            return 0f; // Idle

        input.Normalize();

        if (input.y > 0.5f)
            return 0.25f; // 앞

        if (input.y < -0.5f)
            return 0.5f; // 뒤

        if (input.x < -0.5f)
            return 0.75f; // 왼쪽

        if (input.x > 0.5f)
            return 1f; // 오른쪽

        return 0f;
    }

    private void RotateToMovement()
    {
        Vector3 velocity = _agentMovement.Velocity;
        velocity.y = 0;

        if (velocity.sqrMagnitude < 0.001f)
            return;

        Quaternion target = Quaternion.LookRotation(velocity);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            target,
            8f * Time.deltaTime);

        _renderer.Animator.SetBool(_isGunParam.ParamHash, IsAiming);
    }

    private void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 dir = (hit.point - transform.position);
            dir.y = 0;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion target = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                target,
                _rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleMovement(Vector2 input)
    {
        _movementInput = input;
        _movement.SetMovementDirection(input);
    }

    private void HandleAim(bool isAiming)
    {
        bool finalAim = isAiming;

        if (_stamina != null && !_stamina.CanAim)
            finalAim = false;

        IsAiming = finalAim;

        _agentMovement.SetUseRotation(!finalAim);

        UpdateSpeed();
    }

    private void HandleSprint(bool isSprinting)
    {
        if (_stat != null && !_stat.CanRun())
            isSprinting = false;

        _stat.SetRunning(isSprinting);

        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        float multiplier = 1f;

        if (IsAiming)
            multiplier *= 0.3f;

        if (_stat != null && _stat.IsRunning)
            multiplier *= 1.5f;

        _agentMovement.SetSpeedMultiplier(multiplier);
    }

    public void ForceStopAim()
    {
        IsAiming = false;

        _agentMovement.SetUseRotation(true);

        UpdateSpeed();
    }

    private void OnDestroy()
    {
        PlayerInput.OnMovementChange -= HandleMovement;
        PlayerInput.OnAimAction -= HandleAim;
        PlayerInput.OnSprintAction -= HandleSprint;
    }
}