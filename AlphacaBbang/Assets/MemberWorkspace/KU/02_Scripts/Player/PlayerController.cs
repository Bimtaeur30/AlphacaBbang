using JJH._02_Scripts_Systems.AnimationSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Agent
{
    [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }

    [SerializeField] private AnimParamSO _isGunParam;
    [SerializeField] private AnimParamSO _speedParam;
    [SerializeField] private AnimParamSO _attackXParam;
    [SerializeField] private AnimParamSO _attackYParam;
    [SerializeField] private float _rotationSpeed = 10f;

    [SerializeField] private float _releaseDuration = 1.1f;
    [SerializeField] private float _shortClickThreshold = 0.65f;

    private IRenderer _renderer;
    private AgentMovement _agentMovement;
    private IControllerMovement _movement;

    private PlayerStaminaGaugeSystem _stamina;
    private PlayerStatSystem _stat;

    private Vector2 _movementInput;

    private PlayerAimState _aimState = PlayerAimState.Idle;

    private float _releaseTimer;
    private float _pressTimer;
    private bool _wasPressed;

    private float _prevYaw;
    private bool _forceBlockAim = false;

    public bool IsAiming =>
        _aimState == PlayerAimState.Aiming ||
        _aimState == PlayerAimState.Releasing;

    protected override void Awake()
    {
        base.Awake();

        _movement = GetModule<IControllerMovement>();
        _agentMovement = _movement as AgentMovement;
        _renderer = GetModule<IRenderer>();

        _stamina = GetComponentInChildren<PlayerStaminaGaugeSystem>();
        _stat = GetComponentInChildren<PlayerStatSystem>();

        PlayerInput.OnMovementChange += HandleMovement;
        PlayerInput.OnSprintAction += HandleSprint;
    }

    private void Update()
    {
        HandleAimInput();
        UpdateAimState();

        if (IsAiming)
            RotateToMouse();
        else
            RotateToMovement();

        UpdateAnimation();
    }

    private void HandleAimInput()
    {
        bool isPressed = Mouse.current.rightButton.isPressed;

        if (_forceBlockAim)
            return;

        if (isPressed)
        {
            _pressTimer += Time.deltaTime;
            _wasPressed = true;

            if (_aimState == PlayerAimState.Idle)
            {
                if (_stamina != null && !_stamina.CanAim)
                    return;

                _aimState = PlayerAimState.Aiming;
                _agentMovement.SetUseRotation(false);
                UpdateSpeed();
            }
        }
        else
        {
            if (_wasPressed)
            {
                if (_aimState == PlayerAimState.Aiming)
                {
                    _aimState = PlayerAimState.Releasing;

                    if (_pressTimer < _shortClickThreshold)
                        _releaseTimer = _releaseDuration;
                    else
                        _releaseTimer = 0f;
                }

                _pressTimer = 0f;
                _wasPressed = false;
            }
        }
    }

    private void UpdateAimState()
    {
        if (_aimState == PlayerAimState.Releasing)
        {
            if (_releaseTimer <= 0f)
            {
                _aimState = PlayerAimState.Idle;
                _agentMovement.SetUseRotation(true);
                UpdateSpeed();
                return;
            }

            _releaseTimer -= Time.deltaTime;

            if (_releaseTimer <= 0f)
            {
                _aimState = PlayerAimState.Idle;
                _agentMovement.SetUseRotation(true);
                UpdateSpeed();
            }
        }
    }

    private void HandleMovement(Vector2 input)
    {
        _movementInput = input;
        _movement.SetMovementDirection(input);
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

    private void UpdateAnimation()
    {
        float speed = _agentMovement.Velocity.magnitude;
        float normalized = speed / 8f;

        if (normalized < 0.5f)
            normalized = 0f;

        _renderer.SetFloat(_speedParam.ParamHash, normalized, 0.1f, Time.deltaTime);
        _renderer.SetBool(_isGunParam.ParamHash, IsAiming);

        if (!IsAiming)
        {
            _renderer.SetFloat(_attackXParam.ParamHash, 0f, 0.1f, Time.deltaTime);
            _renderer.SetFloat(_attackYParam.ParamHash, 0f, 0.1f, Time.deltaTime);

            _prevYaw = transform.eulerAngles.y;
            return;
        }

        Vector2 input = _movementInput;

        if (input.sqrMagnitude < 0.01f)
        {
            float currentYaw = transform.eulerAngles.y;
            float delta = Mathf.DeltaAngle(_prevYaw, currentYaw);

            float turnSpeed = delta / Time.deltaTime;

            if (Mathf.Abs(delta) < 0.1f)
                turnSpeed = 0f;

            float fakeX = Mathf.Clamp(turnSpeed * 0.02f, -1f, 1f);

            _renderer.SetFloat(_attackXParam.ParamHash, fakeX, 0.1f, Time.deltaTime);
            _renderer.SetFloat(_attackYParam.ParamHash, 0f, 0.1f, Time.deltaTime);

            _prevYaw = currentYaw;
        }
        else
        {
            UpdateAttackAnimation(input);
            _prevYaw = transform.eulerAngles.y;
        }
    }

    private void UpdateAttackAnimation(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
        {
            _renderer.SetFloat(_attackXParam.ParamHash, 0f, 0.1f, Time.deltaTime);
            _renderer.SetFloat(_attackYParam.ParamHash, 0f, 0.1f, Time.deltaTime);
            return;
        }

        Vector3 worldDir = Quaternion.Euler(0, -45f, 0) * new Vector3(input.x, 0, input.y);
        Vector3 localDir = transform.InverseTransformDirection(worldDir);
        localDir.Normalize();

        _renderer.SetFloat(_attackXParam.ParamHash, localDir.x, 0.1f, Time.deltaTime);
        _renderer.SetFloat(_attackYParam.ParamHash, localDir.z, 0.1f, Time.deltaTime);
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
    }

    private void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 dir = hit.point - transform.position;
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
    public void ForceStopAim()
    {
        _aimState = PlayerAimState.Releasing;
        _releaseTimer = 0f;

        _agentMovement.SetUseRotation(true);
        UpdateSpeed();
    }
    private void OnDestroy()
    {
        PlayerInput.OnMovementChange -= HandleMovement;
        PlayerInput.OnSprintAction -= HandleSprint;
    }
}

public enum PlayerAimState
{
    Idle,
    Aiming,
    Releasing
}