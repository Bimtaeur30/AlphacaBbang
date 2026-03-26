using UnityEngine;

public class AgentMovement : MonoBehaviour, IModule, IControllerMovement
{
    [SerializeField] private float _moveSpeed = 8f, gravity = -9.8f;
    private CharacterController _characterController;

    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;
    private float _verticalVelocity;
    private Vector3 _movementDirection;
    private ModuleOwner _owner;



    public void SetMovementDirection(Vector2 movementInput)
    {
        _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
    }


    private void Update()
    {
        CalculateMovement();
        ApplyGravity();
        Move();
    }

    private void CalculateMovement()
    {
        _velocity = Quaternion.Euler(0, -45f, 0) * _movementDirection;
        _velocity *= _moveSpeed * Time.fixedDeltaTime;


        if (_velocity.sqrMagnitude > 0)
        {
            float rotationSpeed = 8f;
            Quaternion targetRotation = Quaternion.LookRotation(_velocity);
            _owner.transform.rotation = Quaternion.Lerp(transform.parent.rotation,
                targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    private void ApplyGravity()
    {
        if (_verticalVelocity < 0)
            _verticalVelocity = -0.03f;
        else
            _verticalVelocity += gravity * Time.deltaTime;

        _velocity.y = _verticalVelocity;
    }

    private void Move()
    {
        _characterController.Move(_velocity);
    }

    public void Initialize(ModuleOwner owner)
    {
        _owner = owner;
        _characterController = owner.GetComponent<CharacterController>();
    }
}
