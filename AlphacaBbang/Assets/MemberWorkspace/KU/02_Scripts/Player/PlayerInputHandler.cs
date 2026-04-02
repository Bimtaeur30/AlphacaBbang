using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputSO _playerInput;

    private void Awake()
    {
        if(TryGetComponent<PlayerController>(out PlayerController controller))
        {
            _playerInput = controller.PlayerInput;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _playerInput.SetMovement(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _playerInput.SetLook(context.ReadValue<Vector2>());
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
            _playerInput.SetAim(true);
        else if (context.canceled)
            _playerInput.SetAim(false);
    }
}