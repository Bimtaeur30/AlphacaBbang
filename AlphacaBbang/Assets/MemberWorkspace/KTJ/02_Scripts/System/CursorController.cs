using Reflex.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum CursorMode
{
    Default = 0, Gun = 1
}

public class CursorController : MonoBehaviour, IInstaller
{
    [SerializeField] private Image defaultCursor;
    [SerializeField] private Image gunCursor;
    private bool _isCaptured;
    private void Awake()
    {
        ChangeCursorMode(CursorMode.Default);
    }
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

        Vector2 mousePos = Mouse.current.position.ReadValue();
        defaultCursor.rectTransform.position = mousePos;
    }

    private void CaptureCursor()
    {
        _isCaptured = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void ReleaseCursor()
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

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);
    }

    public Image GetGunCursor()
    {
        return gunCursor;
    }

    public void ChangeCursorMode(CursorMode mode)
    {
        int m = (int)mode;
        if (m == 0)
        {
            defaultCursor.gameObject.SetActive(true);
            gunCursor.gameObject.SetActive(false);
        }
        else
        {
            defaultCursor.gameObject.SetActive(false);
            gunCursor.gameObject.SetActive(true);
        }
    }
}