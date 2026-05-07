using UnityEngine;

public class AgentMovement : MonoBehaviour, IModule, IControllerMovement
{
    [field: SerializeField] public float _moveSpeed { get; private set; }

    private Rigidbody _rigidbody;

    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;

    private Vector3 _movementDirection;
    private ModuleOwner _owner;

    private float _currentSpeedMultiplier = 1;
    private bool _useRotation = true;

    public void SetMovementDirection(Vector2 movementInput)
    {
        _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
    }

    public void SetMovementDirection(Vector3 direction)
    {
        direction.y = 0f;
        _movementDirection = direction.normalized;
    }

    private void FixedUpdate()
    {
        CalculateMovement();
        Move();
    }

    private void CalculateMovement()
    {
        Vector3 dir = Quaternion.Euler(0, -45f, 0) * _movementDirection;

        _velocity = dir * _moveSpeed * _currentSpeedMultiplier;

        if (_useRotation && _velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_velocity);
            _owner.transform.rotation = Quaternion.Lerp(
                _owner.transform.rotation,
                targetRotation,
                10f * Time.fixedDeltaTime);
        }
    }

    private void Move()
    {
        Vector3 move = _velocity * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + move);
    }

    public void Initialize(ModuleOwner owner)
    {
        _owner = owner;
        _rigidbody = owner.GetComponent<Rigidbody>();

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void SetUseRotation(bool useRotation)
    {
        _useRotation = useRotation;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _currentSpeedMultiplier = multiplier;
    }
}