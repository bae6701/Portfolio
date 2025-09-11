using UnityEngine;

public class PlayerController : Character, IMovementInputHandler, ISkillInputHandler
{
    private float moveInput = 0f;

    protected override void Initialize()
    {
        base.Initialize();
        defaultFaceRight = true;
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameState.Playing || currentHealth <= 0) return;
        ProcessMovementInput(moveInput);
    }

    public override void ProcessMovementInput(float moveInput)
    {
        base.ProcessMovementInput(moveInput);

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
    }
    public void SetMoveDirection(int direction)
    {
        moveInput = direction;
    }

    public void StopMoving()
    {
        moveInput = 0f;
        ResetDirection();
    }

    void ISkillInputHandler.UseSkill(int skillIndex)
    {
        if (GameManager.instance.currentState != GameState.Playing || currentHealth <= 0) return;
        base.UseSkill(skillIndex);
    }

    protected override void Die()
    {
        Debug.Log("플레이어 사망!");
        GameManager.instance.PlayerLose();
        TriggerDieAnimation();
    }
}
