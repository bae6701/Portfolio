using UnityEngine;

[CreateAssetMenu(fileName = "ParabolicMovement", menuName = "Projectile/Parabolic")]
public class ParabolicMovement : ProjectileMovement
{
    public float speed = 5f;
    public float arcHeight = 6f;

    [Header("궤적 세부 설정")]
    [Tooltip("목표 지점을 지나친 후 투사체가 사라지기까지의 여유 진행률 (1.1 = 10% 더 진행 후)")]
    [SerializeField] private float maxProgress = 1.1f;

    [Tooltip("다음 위치 예측을 위한 미세 진행률 (회전 계산용).")]
    [SerializeField] private float rotationPredictionStep = 0.01f;

    public override void Init(Projectile projectile)
    {
        
    }

    public override void Move(Projectile projectile)
    {
        float distCovered = (Time.time - projectile.StartTime) * speed;
        if (projectile.TotalDist <= 0)
        {
            projectile.ReturnToPool();
            return;
        }
        float flyProgress = distCovered / projectile.TotalDist;

        if (flyProgress > maxProgress)
        {
            projectile.ReturnToPool();
            return;
        }

        Vector3 currentPos = Vector3.Lerp(projectile.StartPoint, projectile.TargetPoint, flyProgress);
        float arc = arcHeight * Mathf.Sin(flyProgress * Mathf.PI);
        currentPos.y += arc;

        float nextProgress = flyProgress + rotationPredictionStep;
        if (nextProgress <= 1f)
        {
            Vector3 nextPos = Vector3.Lerp(projectile.StartPoint, projectile.TargetPoint, nextProgress);
            float nextArc = arcHeight * Mathf.Sin(nextProgress * Mathf.PI);
            nextPos.y += nextArc;

            Vector3 direction = (nextPos - currentPos).normalized;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }
        }

        projectile.transform.position = currentPos;
    }
}
