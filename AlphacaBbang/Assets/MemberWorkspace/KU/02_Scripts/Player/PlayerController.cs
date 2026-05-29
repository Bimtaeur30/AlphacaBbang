using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using JJH._02_Scripts_Systems.AnimationSystems;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
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
    [SerializeField] private float footStepInterval = 0.4f;

    [SerializeField] private CursorController _cursorController;

    [SerializeField] private SoundClipSO equipSoundClip;
    [SerializeField] private EventChannelSO soundChannel;
    [SerializeField] private EventChannelSO playerChannel;


    public bool IsPureAiming => _aimState == PlayerAimState.Aiming;

    private AgentMovement _agentMovement;

    public bool IsCanMoving { get; private set; } = true; //중요하긴회!

    private PlayerSaveData _saveData;
    private PlayerStaminaGaugeSystem _stamina;
    private PlayerStatSystem _stat;
    private WeaponHandleModule weaponHandleModule;

    private Vector2 _movementInput;
    private float _additionalSpeedMultiplier = 1f;
    private float _footStepTimer;
    private bool _isWalkSoundPlaying;

    private PlayerAimState _aimState = PlayerAimState.Idle;

    private PlayerAimState AimState
    {
        get => _aimState;
        set
        {
            if (_aimState == value)
                return;

            var prev = _aimState;
            _aimState = value;

            switch (value)
            {
                case PlayerAimState.Aiming:
                    OnStartAiming();
                    break;

                case PlayerAimState.Idle when prev == PlayerAimState.Releasing:
                    OnStopAiming();
                    break;
            }
        }
    }

    private float _releaseTimer;
    private float _pressTimer;
    private bool _wasPressed;

    private float _prevYaw;
    private bool _forceBlockAim = false;

    private CapsuleCollider _aimCollider;
    private CapsuleCollider _playerCollider;

    public bool IsAiming =>
        _aimState == PlayerAimState.Aiming ||
        _aimState == PlayerAimState.Releasing;

    protected override void Awake()
    {
        base.Awake();

        _agentMovement = GetComponentInChildren<AgentMovement>();

        _stamina = GetComponentInChildren<PlayerStaminaGaugeSystem>();
        _stat = GetComponentInChildren<PlayerStatSystem>();

        _aimCollider = base.Renderer.Animator.gameObject.GetComponentInChildren<CapsuleCollider>();
        _playerCollider = GetComponent<CapsuleCollider>();

        UpdateColliderState();

        _saveData = GetComponent<PlayerSaveData>();
        HealthModule.InitHealth(_stat.MaxHealth);

        PlayerInput.OnMovementChange += HandleMovement;
        PlayerInput.OnSprintAction += HandleSprint;

        playerChannel.AddListener<AgentStopMoveEvent>(PlayerMoveStop);
    }

    private void PlayerMoveStop(AgentStopMoveEvent evt)
    {
        if (evt.Agent != this)
            return;

        IsCanMoving = !evt.IsStop;
        Debug.Log("무빙ㅇ이이이잉잉"+IsCanMoving);

        if (!IsCanMoving)
        {
            _movementInput = Vector2.zero;
            Movement.SetMovementDirection(Vector2.zero);

            _stat?.SetRunning(false);

            ForceStopAim();

            GunHandleModule?.Fire(false);

            base.Renderer.SetFloat(_speedParam.ParamHash, 0f);

            if (_agentMovement != null)
                _agentMovement.SetSpeedMultiplier(0f);
        }
        else
        {
            UpdateSpeed();
        }
    }

    private void Update()
    {
        if (!IsCanMoving)
        {
            //UpdateAnimation();
            return;
        }

        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            playerChannel.RaiseEvent(AgentEvents.AgentStopMoveEvent.Init(this, true));
        }
        UpdateAimState();

        if (IsAiming)
            RotateToMouse();
        else
            RotateToMovement();

        UpdateAnimation();
    }
    private void HandleAimInput(bool isPressed)
    {
        if (!IsCanMoving)
            return;
        if (_forceBlockAim)
            return;

        if (isPressed)
        {
            _pressTimer += Time.deltaTime;
            _wasPressed = true;

            if (AimState == PlayerAimState.Idle)
            {
                if (_stamina != null && !_stamina.CanAim)
                    return;

                if (weaponHandleModule == null ||
                    weaponHandleModule.CurrentWeapon == null)
                    return;

                AimState = PlayerAimState.Aiming;

                _agentMovement.SetUseRotation(false);

                UpdateSpeed();
                UpdateColliderState();
            }
        }
        else
        {
            if (_wasPressed)
            {
                if (AimState == PlayerAimState.Aiming)
                {
                    AimState = PlayerAimState.Releasing;

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
        if (!IsCanMoving)
            return;
        if (AimState != PlayerAimState.Releasing)
            return;

        if (_releaseTimer <= 0f)
        {
            AimState = PlayerAimState.Idle;

            _agentMovement.SetUseRotation(true);

            UpdateSpeed();
            UpdateColliderState();

            return;
        }

        _releaseTimer -= Time.deltaTime;

        if (_releaseTimer <= 0f)
        {
            AimState = PlayerAimState.Idle;

            _agentMovement.SetUseRotation(true);

            UpdateSpeed();
            UpdateColliderState();
        }
    }

    private void HandleMovement(Vector2 input)
    {
        if (!IsCanMoving)
        {
            // _movementInput = Vector2.zero;
            // Movement.SetMovementDirection(Vector2.zero);
            return;
        }

        _movementInput = input;

        Vector3 dir = Quaternion.Euler(0, 45f, 0)
            * new Vector3(input.x, 0, input.y);

        Movement.SetMovementDirection(new Vector2(dir.x, dir.z));
    }

    private void HandleSprint(bool isSprinting)
    {
        if (!IsCanMoving)
            return;

        if (_stat != null && !_stat.CanRun())
            isSprinting = false;

        _stat.SetRunning(isSprinting);

        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (!IsCanMoving)
            return;
        float multiplier = 1f;

        if (IsAiming)
            multiplier *= 0.3f;

        if (_stat != null && _stat.IsRunning)
            multiplier *= 1.5f;

        multiplier *= _additionalSpeedMultiplier;

        _agentMovement.SetSpeedMultiplier(multiplier);
    }

    IEnumerator PercentAddPlayerMoveSpeed(int val, float time)
    {
        float addMultiplier = 1f + (val / 100f);

        _additionalSpeedMultiplier *= addMultiplier;

        UpdateSpeed();

        yield return new WaitForSeconds(time);

        _additionalSpeedMultiplier /= addMultiplier;

        UpdateSpeed();
    }

    private void HandlePlayerMoveSpeed(AddPlayerMoveSpeed evt)
    {
        StartCoroutine(PercentAddPlayerMoveSpeed(evt.val, evt.time));
    }

    private void UpdateAnimation()
    {
        if (!IsCanMoving)
            return;
        float speed = _agentMovement.Velocity.magnitude;
        float normalized = speed / 8f;

        if (normalized < 0.5f)
            normalized = 0f;

        base.Renderer.SetFloat(_speedParam.ParamHash, normalized, 0.1f, Time.deltaTime);
        base.Renderer.SetBool(_isGunParam.ParamHash, IsAiming);

        if (!IsAiming)
        {
            base.Renderer.SetFloat(_attackXParam.ParamHash, 0f, 0.1f, Time.deltaTime);
            base.Renderer.SetFloat(_attackYParam.ParamHash, 0f, 0.1f, Time.deltaTime);

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

            base.Renderer.SetFloat(_attackXParam.ParamHash, fakeX, 0.1f, Time.deltaTime);
            base.Renderer.SetFloat(_attackYParam.ParamHash, 0f, 0.1f, Time.deltaTime);

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
        if (!IsCanMoving)
            return;
        if (input.sqrMagnitude < 0.01f)
        {
            base.Renderer.SetFloat(_attackXParam.ParamHash, 0f, 0.1f, Time.deltaTime);
            base.Renderer.SetFloat(_attackYParam.ParamHash, 0f, 0.1f, Time.deltaTime);

            return;
        }

        //Vector3 worldDir = Quaternion.Euler(0, -45f, 0) * new Vector3(input.x, 0, input.y);
        Vector3 worldDir = new Vector3(input.x, 0, input.y);

        Vector3 localDir = transform.InverseTransformDirection(worldDir);

        localDir.Normalize();

        base.Renderer.SetFloat(_attackXParam.ParamHash, localDir.x, 0.1f, Time.deltaTime);
        base.Renderer.SetFloat(_attackYParam.ParamHash, localDir.z, 0.1f, Time.deltaTime);
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
        Vector2 mousePos = CrossHairModule.CHMousePos;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 dir = hitPoint - transform.position;

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
        AimState = PlayerAimState.Releasing;

        _releaseTimer = 0f;

        _agentMovement.SetUseRotation(true);

        UpdateSpeed();
        UpdateColliderState();
    }

    public void RefreshMovementSpeed()
    {
        UpdateSpeed();
    }

    private void UpdateColliderState()
    {
        bool isAim = IsAiming;

        if (_playerCollider != null)
            _playerCollider.enabled = !isAim;

        if (_aimCollider != null)
            _aimCollider.enabled = isAim;
    }

    private void OnDestroy()
    {
        PlayerInput.OnMovementChange -= HandleMovement;
        PlayerInput.OnSprintAction -= HandleSprint;
    }

    #region 총 관련 코드

    #region 입력

    [SerializeField] private PlayerInputSO_KTJ playerInputSO;

    #endregion

    #region 모듈

    public PlayerGunHandleModule GunHandleModule { get; private set; }
    public CrossHairModule CrossHairModule { get; private set; }

    #endregion

    #region 글로벌 변수

    public Camera MainCam { get; private set; }
    public Vector2 Forward { get; private set; }

    #endregion

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        GunHandleModule = GetModule<PlayerGunHandleModule>();
        Debug.Assert(GunHandleModule != null, "GunHandleModule is null");

        CrossHairModule = GetModule<CrossHairModule>();
        Debug.Assert(CrossHairModule != null, "CrossHairModule is null");

        weaponHandleModule = GetModule<WeaponHandleModule>();
        Debug.Assert(weaponHandleModule != null, "WeaponHandleModule is null");

        MainCam = Camera.main;
        Debug.Assert(MainCam != null, "MainCam is null");
    }

    private void OnEnable()
    {
        if (playerInputSO == null)
            return;

        playerInputSO.OnAimEvent += HandleAimKey;
        playerInputSO.OnFireEvent += HandleFireKey;
    }

    private void OnDisable()
    {
        if (playerInputSO == null)
            return;

        playerInputSO.OnAimEvent -= HandleAimKey;
        playerInputSO.OnFireEvent -= HandleFireKey;
    }

    private void HandleAimKey(bool isPressed)
    {
        HandleAimInput(isPressed);
        if (GunHandleModule == null)
            return;

        //GunHandleModule.Aim(isPressed);
    }

    private void HandleFireKey(bool isPressed)
    {
        if (!IsCanMoving)
            return;
        if (GunHandleModule == null)
            return;

        GunHandleModule.Fire(isPressed);
    }

    private void OnStartAiming()
    {
        if (!IsCanMoving)
            return;
        _cursorController.ChangeCursorMode(CursorMode.Gun);
        soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(equipSoundClip, this.transform));
        GunHandleModule.Aim(true);
    }

    private void OnStopAiming()
    {
        if (!IsCanMoving)
            return;
        _cursorController.ChangeCursorMode(CursorMode.Default);

        GunHandleModule.Aim(false);
        GunHandleModule.Fire(false);
    }

    #endregion
}

public enum PlayerAimState
{
    Idle,
    Aiming,
    Releasing
}