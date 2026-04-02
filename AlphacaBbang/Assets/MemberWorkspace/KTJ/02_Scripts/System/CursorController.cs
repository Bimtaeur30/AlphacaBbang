using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    private bool _isCaptured;

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CaptureCursor();
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReleaseCursor();
        }
    }

    public void CaptureCursor()
    {
        _isCaptured = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void ReleaseCursor()
    {
        _isCaptured = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            ReleaseCursor();
            return;
        }

        if (_isCaptured)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }
}