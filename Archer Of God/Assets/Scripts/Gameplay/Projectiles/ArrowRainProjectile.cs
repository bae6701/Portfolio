using UnityEngine;

public class ArrowRainProjectile : Projectile
{
    [Header("화살비 설정")]
    [Tooltip("실제로 떨어질 화살 프리팹")]
    [SerializeField] private GameObject projectileToSpawn;
    [Tooltip("생성할 총 화살 수")]
    [SerializeField] private int numberOfProjectiles = 5;
    [Tooltip("각 화살 사이의 좌우 간격")]
    [SerializeField] private float arrowSpacing = 1.5f;
    [Tooltip("화살이 생성될 높이")]
    [SerializeField] private float spawnHeight = 5f;

    void OnEnable()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public new void Launch(Transform target, bool isFacingRight)
    {
        if (projectileToSpawn == null)
        {
            Debug.LogError("생성할 투사체가 지정되지 않았습니다!");
            ReturnToPool();
            return;
        }

        FireArrowLine(target);
    }

    private void FireArrowLine(Transform target)
    {
        Vector3 targetCenter = target.position;

        // 전체 화살 라인의 총 너비를 계산합니다.
        // 예: 화살 5개, 간격 1.5 -> 총 너비 = 1.5 * 4 = 6
        float totalWidth = arrowSpacing * (numberOfProjectiles - 1);

        // 첫 번째 화살이 생성될 X 좌표를 계산하여 전체 라인이 타겟 중앙에 오도록 합니다.
        float startX = targetCenter.x - (totalWidth / 2f);

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // 각 화살의 X 위치를 계산합니다.
            float spawnX = startX + (i * arrowSpacing);
            Vector3 spawnPosition = new Vector3(spawnX, targetCenter.y + spawnHeight, 0);

            Poolable poolable = PoolManager.Instance.Pop(projectileToSpawn);
            GameObject arrow = poolable.gameObject;
            arrow.transform.position = spawnPosition;

            Projectile projectileComponent = arrow.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                // 각 화살의 낙하를 시작시킵니다.
                projectileComponent.Launch(target, true);
            }
        }
        ReturnToPool();
    }
}
