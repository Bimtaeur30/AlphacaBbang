using UnityEngine;

public interface IControllerMovement
{
    public void SetMovementDirection(Vector2 movementInput);
    public void SetMovementDirection(Vector3 direction);
}