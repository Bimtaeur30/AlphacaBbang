using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaGaugeSystem : MonoBehaviour
{
    private PlayerInputSO _playerInput;

    private bool _isAiming;
    private bool _canAim = true;
    public bool CanAim => _canAim;

    [SerializeField] private Image _gaugeImage;

    public float CurrentGauge { get; private set; }

    [SerializeField] private float _gaugeMaxTime = 10f;
    [SerializeField] private float _minAimCooldown = 2f;
    [SerializeField] private float _useSpeed = 1f;
    private float _chargeSpeed = 1f;
    private float _cooldownTimer = 0f;

    private void Awake()
    {
        _playerInput = GetComponentInParent<PlayerController>().PlayerInput;

        CurrentGauge = _gaugeMaxTime;
    }

    private void OnEnable()
    {
        _playerInput.OnAim += HandleAim;
    }

    private void OnDisable()
    {
        _playerInput.OnAim -= HandleAim;
    }

    private void HandleAim(bool isAiming)
    {
        if (!_canAim)
            return;

        _isAiming = isAiming;
    }

    private void Update()
    {
        UpdateGauge();
        UpdateCooldown();
        UpdateUI();
        if (!_canAim)
            _gaugeImage.color = Color.red;
        else
            _gaugeImage.color = Color.yellow;
    }

    private void UpdateGauge()
    {
        float delta = _useSpeed * Time.deltaTime;

        if (_isAiming)
        {
            CurrentGauge -= delta;

            if (CurrentGauge <= 0f)
            {
                CurrentGauge = 0f;

                _isAiming = false;
                _canAim = false;
                _cooldownTimer = _minAimCooldown;

                _playerInput.SetAim(false);
            }
        }
        else
        {
            CurrentGauge += _chargeSpeed * Time.deltaTime;
        }

        CurrentGauge = Mathf.Clamp(CurrentGauge, 0, _gaugeMaxTime);
    }

    private void UpdateCooldown()
    {
        if (_canAim)
            return;

        _cooldownTimer -= Time.deltaTime;

        if (_cooldownTimer <= 0f)
        {
            _canAim = true;
        }
    }

    private void UpdateUI()
    {
        float fill = CurrentGauge / _gaugeMaxTime;

        _gaugeImage.fillAmount = Mathf.Lerp(
            _gaugeImage.fillAmount,
            fill,
            10f * Time.deltaTime);
    }
}