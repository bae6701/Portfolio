using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// 파일 이름을 BotController에서 AIController로 변경하셨으므로 클래스 이름도 맞춰줍니다.
public class AIController : Character
{
    private CancellationTokenSource _cts;

    [Header("AI 행동 패턴 설정")]
    [Tooltip("기본 공격과 스킬 사용 사이의 최소 대기 시간")]
    [SerializeField] private float minActionDelay = 0.5f;
    [Tooltip("기본 공격과 스킬 사용 사이의 최대 대기 시간")]
    [SerializeField] private float maxActionDelay = 1.5f;

    [Header("AI 움직임 설정")]
    [Tooltip("한 번 이동할 때의 최소 지속 시간")]
    [SerializeField] private float minMoveDuration = 0.8f;
    [Tooltip("한 번 이동할 때의 최대 지속 시간")]
    [SerializeField] private float maxMoveDuration = 2.0f;

    protected override void Initialize()
    {
        base.Initialize();
        // Bot은 오른쪽에 위치하여 왼쪽을 바라보는 것이 기본값입니다.
        defaultFaceRight = false;
        _cts = new CancellationTokenSource();
    }

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance.currentState == GameState.Playing)
        {
            StartAIBehavior();
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void StartAIBehavior()
    {
        // AI 행동 루프를 시작합니다.
        AIActionLoopAsync(_cts.Token).Forget();
    }

    // AI의 메인 행동 루프: [이동 -> 공격 -> 스킬] 패턴을 반복합니다.
    private async UniTaskVoid AIActionLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && currentHealth > 0)
        {
            // 1. 무작위 방향으로, 무작위 시간 동안 이동합니다.
            await PerformRandomMoveAsync(token);
            if (token.IsCancellationRequested) return;

            // 2. 타겟을 바라보고 기본 공격을 합니다.
            await PerformAttackAsync(token);
            if (token.IsCancellationRequested) return;

            // 3. 사용 가능한 스킬이 있다면 타겟을 바라보고 사용합니다.
            await PerformSkillAsync(token);
            if (token.IsCancellationRequested) return;
        }
    }

    // 무작위 방향으로 짧게 이동
    private async UniTask PerformRandomMoveAsync(CancellationToken token)
    {
        float randomDirection = (UnityEngine.Random.value < 0.5f) ? -1f : 1f;
        float randomDuration = UnityEngine.Random.Range(minMoveDuration, maxMoveDuration);

        ProcessMovementInput(randomDirection);
        await UniTask.Delay(TimeSpan.FromSeconds(randomDuration), cancellationToken: token);
        ProcessMovementInput(0); // 이동 정지
    }

    public override void ProcessMovementInput(float moveInput)
    {
        base.ProcessMovementInput(moveInput);

        if (moveInput > 0 && isFacingRight) Flip();
        else if (moveInput < 0 && !isFacingRight) Flip();
    }

    // 기본 공격 수행
    private async UniTask PerformAttackAsync(CancellationToken token)
    {
        // 공격 전, 반드시 타겟을 바라보도록 방향을 보정합니다.
        FaceTarget();

        BasicAttack();

        // 기본 공격 애니메이션이 끝날 때까지 대기합니다. (이벤트 기반으로 개선 가능)
        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);

        // 다음 행동 전 잠시 대기
        float delay = UnityEngine.Random.Range(minActionDelay, maxActionDelay);
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
    }

    // 스킬 사용
    private async UniTask PerformSkillAsync(CancellationToken token)
    {
        int skillIndex = GetAvailableSkillIndex();
        // 사용 가능한 스킬이 있을 때만 행동합니다.
        if (skillIndex != -1)
        {
            // 스킬 사용 전, 반드시 타겟을 바라보도록 방향을 보정합니다.
            FaceTarget();

            UseSkill(skillIndex);

            // 스킬 애니메이션이 끝날 때까지 (IsUsingSkill이 false가 될 때까지) 기다립니다.
            await UniTask.WaitUntil(() => !_isUsingSkill, cancellationToken: token);

            // 다음 행동 전 잠시 대기
            float delay = UnityEngine.Random.Range(minActionDelay, maxActionDelay);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
        }
    }

    // 사용 가능한 스킬 중 하나의 인덱스를 무작위로 반환 (없으면 -1)
    private int GetAvailableSkillIndex()
    {
        List<int> availableSkills = new List<int>();
        for (int i = 0; i < skills.Count; i++)
        {
            if (!IsSkillOnCooldown(i))
            {
                availableSkills.Add(i);
            }
        }

        if (availableSkills.Count > 0)
        {
            return availableSkills[UnityEngine.Random.Range(0, availableSkills.Count)];
        }
        return -1;
    }

    // 공격 또는 스킬 사용 전에 타겟을 바라보도록 방향 전환
    private void FaceTarget()
    {
        if (_target == null) return;

        bool shouldFaceRight = (_target.transform.position.x < transform.position.x);

        if (isFacingRight != shouldFaceRight)
        {
            Flip();
        }
    }

    protected override void Die()
    {
        Debug.Log("Bot 사망!");
        _cts?.Cancel();
        TriggerDieAnimation();
        GameManager.instance.PlayerWin();
    }
}