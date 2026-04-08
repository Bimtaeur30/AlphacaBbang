using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/PlayerInputSO")]
public class PlayerInputSO : ScriptableObject
{
    public event Action<Vector2> OnMovementChange;
    public event Action<Vector2> OnLookChange;
    public event Action<bool> OnAim;
    public event Action<bool> OnSprint;

    public void SetMovement(Vector2 movement)
    {
        OnMovementChange?.Invoke(movement);
    }

    public void SetLook(Vector2 look)
    {
        OnLookChange?.Invoke(look);
    }

    public void SetAim(bool isAiming)
    {
        OnAim?.Invoke(isAiming);
    }

    public void SetSprint(bool isSprinting)
    {
        OnSprint?.Invoke(isSprinting);
    }
}