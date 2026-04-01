using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerInputSO playerInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        playerInput.SetMovement(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        playerInput.SetLook(context.ReadValue<Vector2>());
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
            playerInput.SetAim(true);
        else if (context.canceled)
            playerInput.SetAim(false);
    }
}