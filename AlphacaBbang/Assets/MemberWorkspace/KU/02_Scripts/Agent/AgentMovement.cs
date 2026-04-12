using UnityEngine;

public class AgentMovement : MonoBehaviour, IModule, IControllerMovement
{
    [field: SerializeField] public float _moveSpeed { get; private set; }
    [SerializeField] private float gravity = -9.8f;
    private CharacterController _characterController;

    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;
    private float _verticalVelocity;
    private Vector3 _movementDirection;
    private ModuleOwner _owner;

    private float _currentSpeedMultiplier = 1;
    private bool _useRotation = true;

    private bool _useLocalMovement = false;


    public void SetMovementDirection(Vector2 movementInput)
    {
        _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
    }

    public void SetMovementDirection(Vector3 direction)
    {
        direction.y = 0f;
        _movementDirection = direction.normalized;
    }

    private void Update()
    {
        CalculateMovement();
        ApplyGravity();
        Move();
    }

    private void CalculateMovement()
    {
        Vector3 dir = new Vector3(_movementDirection.x, 0, _movementDirection.z);

        if (_useLocalMovement)
        {
            dir = _owner.transform.TransformDirection(dir);
        }
        else
        {
            dir = Quaternion.Euler(0, -45f, 0) * dir;
        }

        _velocity = dir * _moveSpeed * _currentSpeedMultiplier;

        if (_useRotation && _velocity.sqrMagnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_velocity);
            _owner.transform.rotation = Quaternion.Lerp(
                _owner.transform.rotation,
                targetRotation,
                8f * Time.deltaTime);
        }
    }


    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0)
                _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }

        _velocity.y = _verticalVelocity;
    }

    private void Move()
    {
        _characterController.Move(_velocity * Time.deltaTime);
    }

    public void Initialize(ModuleOwner owner)
    {
        _owner = owner;
        _characterController = owner.GetComponent<CharacterController>();
    }


    public void SetUseRotation(bool useRotation)
    {
        _useRotation = useRotation;
    }
    public void SetSpeedMultiplier(float multiplier)
    {
        _currentSpeedMultiplier = multiplier;
    }

    public void SetUseLocalMovement(bool value)
    {
        _useLocalMovement = value;
    }
}
