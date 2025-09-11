using UnityEngine;

public interface IMovementInputHandler
{
    void SetMoveDirection(int direction);
    void StopMoving();
}