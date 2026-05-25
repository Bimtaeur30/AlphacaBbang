using System;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "PlayerInputSO", menuName = "KTJ/Input/PlayerInputSO")]
public class PlayerInputSO_KTJ : ScriptableObject, Controls.IGunActions
{
    public event Action<bool> OnAimEvent;
    public event Action<bool> OnFireEvent;

    private Controls _controls;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Gun.SetCallbacks(this);
        }
        _controls.Gun.Enable();
    }

    private void OnDisable()
    {
        _controls.Gun.Disable();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAimEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnAimEvent?.Invoke(false);
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnFireEvent?.Invoke(false);
        }
    }
}
