using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour
{
    [Header("능력치")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("컴포넌트")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;
    protected Transform unitRoot;

    [Header("전투 설정")]
    private float _currentMoveInput = 0f;
    [SerializeField] protected float moveSpeed = 8f;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected GameObject basicAttackPrefab;

    [Header("스킬")]
    public List<Skill> skills = new List<Skill>();
    private bool[] _isSkillOnCooldown;
    public bool IsSkillOnCooldown(int skillIndex)
    {
        if (_isSkillOnCooldown == null || skillIndex < 0 || skillIndex >= _isSkillOnCooldown.Length)
        {
            return true; // 잘못된 인덱스이거나 배열이 초기화되지 않았다면 쿨타임으로 간주
        }
        return _isSkillOnCooldown[skillIndex];
    }
    protected bool _isUsingSkill = false;
    private Skill _currentSkill;
    private bool _isFrozen = false;
    private GameObject _freezeEffectInstance;

    [Header("이동 범위")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    public event Action<float, float> OnHealthChanged;
    public event Action<int, float> OnSkillUsed;
    private Action _onAttackAction;

    protected Character _target;
    public Character GetTarget() { return _target; }
    public void SetTarget(Character target)
    {
        _target = target;
    }

    protected bool isFacingRight = true;
    protected bool defaultFaceRight = true;
    private bool _isGrounded;
    private static readonly int hashIsRunning = Animator.StringToHash("isRunning");
    private static readonly int hashAttackTrigger = Animator.StringToHash("attack");
    private static readonly int hashDieTrigger = Animator.StringToHash("die");

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        unitRoot = transform.Find("UnitRoot");
        currentHealth = maxHealth;

        _isSkillOnCooldown = new bool[skills.Count];
    }

    protected virtual void Start()
    {
        Initialize();

        isFacingRight = defaultFaceRight;
        if ((isFacingRight && transform.localScale.x < 0) || (!isFacingRight && transform.localScale.x > 0))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }

    protected virtual void Initialize() { }

    protected virtual void FixedUpdate()
    {
        if (currentHealth <= 0) return;

        float targetVelocityX = 0f;

        if (!_isUsingSkill && !_isFrozen && _isGrounded)
        {
            targetVelocityX = _currentMoveInput * moveSpeed;
        }

        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        Vector2 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        rb.position = clampedPosition; 
    }

    public virtual void ProcessMovementInput(float moveInput)
    {
        _currentMoveInput = moveInput;

        if (_isUsingSkill || _isFrozen || !_isGrounded)
        {
            animator.SetBool(hashIsRunning, false);
            return;
        }
        animator.SetBool(hashIsRunning, moveInput != 0);

    }
    

    public void ResetDirection()
    {
        if (isFacingRight != defaultFaceRight)
        {
            Flip();
        }
    }

    public void UseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Count || skills[skillIndex] == null) return;

        if (IsSkillOnCooldown(skillIndex) || _isUsingSkill || _isFrozen)
        {
           //Debug.Log($"{skills[skillIndex].skillName}은 현재 사용할 수 없습니다.");
            return;
        }

        _isUsingSkill = true;
        _currentSkill = skills[skillIndex];

        _onAttackAction = _currentSkill.GetAttackAction(this);
        OnSkillUsed?.Invoke(skillIndex, skills[skillIndex].cooldown);
        _currentSkill.Activate(this);
        StartCooldown(skillIndex).Forget();
    }

    public void EndSkillUsage()
    {
        if (_isUsingSkill)
        {
            _isUsingSkill = false;
            _currentSkill = null;
        }
    }
    private async UniTaskVoid StartCooldown(int skillIndex)
    {
        _isSkillOnCooldown[skillIndex] = true;
        await UniTask.Delay(TimeSpan.FromSeconds(skills[skillIndex].cooldown));
        _isSkillOnCooldown[skillIndex] = false;
    }

    public void BasicAttack()
    {
        if (basicAttackPrefab == null) return;
        FireProjectile(basicAttackPrefab);
    }

    public void SkillAttack()
    {
        if (_currentSkill == null || _currentSkill.projectilePrefab == null)
        {
            Debug.LogWarning("현재 스킬에 투사체가 지정되지 않아 기본 공격을 대신 발사합니다.");
            BasicAttack();
            return;
        }
        _onAttackAction?.Invoke();
    }

    public void FireProjectile(GameObject projectilePrefab)
    {
        if (_target == null || _target.currentHealth <= 0) return;

        Poolable poolable = PoolManager.Instance.Pop(projectilePrefab);
        if (poolable == null) return;

        GameObject projGO = poolable.gameObject;
        projGO.transform.position = firePoint.position;

        Projectile projectile = projGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Launch(_target.transform, isFacingRight);
        }
        else
        {
            Debug.LogError("프리팹에 Projectile 컴포넌트가 없습니다.", projectilePrefab);
            PoolManager.Instance.Push(poolable);
        }
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        unitRoot.localScale = new Vector3(unitRoot.localScale.x * -1, unitRoot.localScale.y, unitRoot.localScale.z);
    }

    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void TriggerAnimation(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName)) return;

        if (animator == null)
        {
            Debug.LogWarning($"{gameObject.name}: Animator가 없어 '{triggerName}' 애니메이션을 재생할 수 없습니다.");
            return;
        }

        animator.SetTrigger(triggerName);
    }
    protected void TriggerDieAnimation()
    {
        rb.linearVelocity = Vector2.zero; 
        GetComponent<Collider2D>().enabled = false; 
        animator.SetTrigger(hashDieTrigger);
    }

    protected abstract void Die();

    public async UniTaskVoid Freeze(float duration, GameObject freezeVfxPrefab)
    {
        if (_isFrozen) return; // 이미 얼어있다면 중복 실행 방지

        _isFrozen = true;

        // 얼음 이펙트 생성
        if (freezeVfxPrefab != null)
        {
            _freezeEffectInstance = Instantiate(freezeVfxPrefab, transform.position, Quaternion.identity, transform);
        }

        // rb의 물리적 움직임 완전 정지
        rb.linearVelocity = Vector2.zero;
        _currentMoveInput = 0;
        animator.SetBool(hashIsRunning, false);

        if (animator != null)
        {
            animator.speed = 0f;
        }

        // 지정된 시간만큼 대기
        await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: false);

        // 얼음 상태 해제
        if (_freezeEffectInstance != null)
        {
            Destroy(_freezeEffectInstance);
        }
        _isFrozen = false;

        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

    public void ApplyVerticalImpulse(float force)
    {
        if (rb == null)
        {
            Debug.LogWarning($"{gameObject.name}: Rigidbody2D가 없어 수직 힘을 적용할 수 없습니다.");
            return;
        }
        _isGrounded = false;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 상대방의 태그가 "Ground"이고, 현재 강제 점프 상태일 때
        if (!_isGrounded && collision.gameObject.CompareTag("Ground"))
        {
            // 착지했으므로 공중 상태 플래그를 해제합니다.
            _isGrounded = true;
        }
    }
}

