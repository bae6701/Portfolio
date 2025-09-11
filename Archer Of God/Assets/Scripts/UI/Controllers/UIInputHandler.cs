using UnityEngine;

public class UIInputHandler : MonoBehaviour
{
    [SerializeField] private UI_EventHandler leftMoveButtonHandler;
    [SerializeField] private UI_EventHandler rightMoveButtonHandler;
    [SerializeField] private PlayerController playerController;
    void Start()
    {
        IMovementInputHandler movementHandler = playerController;
        if (movementHandler == null) return;

        leftMoveButtonHandler.onPointerDown += (_) => movementHandler.SetMoveDirection(-1);
        leftMoveButtonHandler.onPointerUp += (_) => movementHandler.StopMoving();

        rightMoveButtonHandler.onPointerDown += (_) => movementHandler.SetMoveDirection(1);
        rightMoveButtonHandler.onPointerUp += (_) => movementHandler.StopMoving();
    }
}
