using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Poolable), typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private enum State { Flying, Stuck }
    private State _currentState;

    [Header("투사체 설정")]
    public float damage = 10f;
    public float lifetime = 8f;
    public float fadeDuration = 2f;
    public Vector3 StartPoint { get; private set; }
    public Vector3 TargetPoint { get; private set; }
    public Vector3 Direction { get; private set; }
    public float TotalDist { get; private set; }
    public float StartTime { get; private set; }
    public ProjectileMovement movement;

    private string _targetTag;
    private Poolable _poolable;

    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private CancellationTokenSource _lifetimeSource;

    void Awake()
    {
        _poolable = GetComponent<Poolable>();
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
    }

    public void Launch(Transform target, bool isFacingRight)
    {

        _lifetimeSource?.Cancel();
        _lifetimeSource = new CancellationTokenSource();

        _currentState = State.Flying;
        _collider.enabled = true;

        StartPoint = transform.position;

        Vector3 aimPoint;

        if (target != null)
        {
            aimPoint = target.position;

            if (movement.targetHeightAdjustment != 0f)
            {
                aimPoint.y += movement.targetHeightAdjustment;
            }
        }
        else
        {
            // 타겟이 없을 경우의 안전 장치
            aimPoint = StartPoint + (isFacingRight ? Vector3.right : Vector3.left) * 20f;
        }

        TargetPoint = aimPoint;
        StartTime = Time.time;
        Direction = (TargetPoint - StartPoint).normalized;
        TotalDist = Vector3.Distance(StartPoint, TargetPoint);

        Color color = _spriteRenderer.color;
        color.a = 1f;
        _spriteRenderer.color = color;

        _targetTag = target.tag;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);

        movement?.Init(this);

        HandleLifetime(_lifetimeSource.Token).Forget();
    }

    void Update()
    {
        if (_currentState == State.Flying)
        {
            movement?.Move(this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_currentState != State.Flying) return;

        if (other.CompareTag(_targetTag))
        {
            Character targetCharacter = other.GetComponent<Character>();
            if (targetCharacter != null)
            {
                ApplyEffect(targetCharacter);
                targetCharacter.TakeDamage(damage);
            }
            ReturnToPool();
        }
        else if (other.CompareTag("Ground"))
        {
            _currentState = State.Stuck;
            _collider.enabled = false;
            FadeOutAndReturn(_lifetimeSource.Token).Forget();
        }
    }

    protected virtual void ApplyEffect(Character target)
    {
        // 기본 투사체는 특별한 효과 없음
    }

    private async UniTaskVoid HandleLifetime(CancellationToken token)
    {
        bool cancelled = await UniTask.Delay(TimeSpan.FromSeconds(lifetime), ignoreTimeScale: false, cancellationToken: token).SuppressCancellationThrow();
        if (!cancelled && _currentState == State.Flying)
        {
            FadeOutAndReturn(token).Forget();
        }
    }

    private async UniTaskVoid FadeOutAndReturn(CancellationToken token)
    {
        float timer = 0f;
        Color startColor = _spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (timer < fadeDuration)
        {
            if (token.IsCancellationRequested) return;

            await UniTask.Yield(PlayerLoopTiming.Update, token);
            timer += Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
        }

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_poolable != null && _poolable.IsUsing)
        {
            _lifetimeSource?.Cancel();
            _lifetimeSource?.Dispose();
            _lifetimeSource = null;
            PoolManager.Instance.Push(_poolable);
        }
    }

    void OnDisable()
    {
        _lifetimeSource?.Cancel();
        _lifetimeSource?.Dispose();
        _lifetimeSource = null;
    }
}
