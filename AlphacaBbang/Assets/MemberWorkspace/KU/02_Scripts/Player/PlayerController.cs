using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Agent
{
    [field: SerializeField] public PlayerInputSO playerInput { get; private set; }

    private IControllerMovement _movement;

    protected override void Awake() 
    {
        base.Awake();

        _movement = GetModule<IControllerMovement>();

        playerInput.OnMovementChange += HandleMovement;
    }

    private void HandleMovement(Vector2 input)
    {
        _movement.SetMovementDirection(input);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        playerInput.SetMovement(input);
    }

    private void OnDestroy()
    {
        playerInput.OnMovementChange -= HandleMovement;
    }
}
