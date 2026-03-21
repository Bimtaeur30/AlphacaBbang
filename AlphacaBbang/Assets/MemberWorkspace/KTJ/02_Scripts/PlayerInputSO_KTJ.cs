using System;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "PlayerInputSO", menuName = "KTJ/Input/PlayerInputSO")]
public class PlayerInputSO_KTJ : ScriptableObject, Controls.IGunActions
{
    public event Action OnAimKeyPressed;
    public event Action OnAimKeyCanceled;

    public event Action OnFireKeyPressed;
    public event Action OnFireKeyCanceled;

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

    public void OnOnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAimKeyPressed?.Invoke();
        }
        else if (context.canceled)
        {
            OnAimKeyCanceled?.Invoke();
        }
    }

    public void OnOnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnFireKeyPressed?.Invoke();
        }
        else if (context.canceled)
        {
            OnFireKeyCanceled?.Invoke();
        }
    }
}
