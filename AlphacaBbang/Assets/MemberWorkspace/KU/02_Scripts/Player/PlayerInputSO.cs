using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/PlayerInputSO")]
public class PlayerInputSO : ScriptableObject, PlayerInputSystem.IPlayerActions
{
    public event Action<Vector2> OnMovementChange;
    public event Action<Vector2> OnLookChange;
    public event Action<bool> OnAimAction;
    public event Action<bool> OnSprintAction;

    private PlayerInputSystem _controls;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new PlayerInputSystem();
            _controls.Player.SetCallbacks(this);
        }

        _controls.Player.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        OnMovementChange?.Invoke(movement);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        OnLookChange?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
            OnAimAction?.Invoke(true);
        else if (context.canceled)
            OnAimAction?.Invoke(false);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            OnSprintAction?.Invoke(true);
        else if (context.canceled)
            OnSprintAction?.Invoke(false);
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }
}