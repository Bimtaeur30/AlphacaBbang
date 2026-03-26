using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/PlayerInputSO")]
public class PlayerInputSO : ScriptableObject
{
    public event Action<Vector2> OnMovementChange;
}
