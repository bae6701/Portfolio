using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New ArrowRain", menuName = "Skills/ArrowRain")]
public class Skill_ArrowRain : Skill
{
    [Header("화살비 스킬 설정")]
    [Tooltip("생성할 총 화살 수")]
    public int numberOfArrows = 5;
    [Tooltip("각 화살 사이의 좌우 간격")]
    public float arrowSpacing = 1.5f;
    [Tooltip("화살이 생성될 높이")]
    public float spawnHeight = 3.6f;

    [Header("애니메이션")]
    [Tooltip("하늘을 조준하는 애니메이션의 트리거 이름")]
    public string animationTriggerName = "arrowRain";

    public override void Activate(Character caster)
    {
        if (caster == null) return;
        caster.TriggerAnimation(animationTriggerName);
    }

    public override Action GetAttackAction(Character caster)
    {
        return () =>
        {
            Vector3 targetCenter = caster.GetTarget().transform.position;
            float totalWidth = arrowSpacing * (numberOfArrows - 1);
            float startX = targetCenter.x - (totalWidth / 2f);

            for (int i = 0; i < numberOfArrows; i++)
            {
                float spawnX = startX + (i * arrowSpacing);
                Vector3 spawnPosition = new Vector3(spawnX, targetCenter.y + spawnHeight, 0);

                Poolable poolable = PoolManager.Instance.Pop(projectilePrefab);
                GameObject arrow = poolable.gameObject;
                arrow.transform.position = spawnPosition;

                Projectile projectileComponent = arrow.GetComponent<Projectile>();
                if (projectileComponent != null)
                {
                    projectileComponent.Launch(caster.GetTarget().transform, true);
                }
            }
        };
    }
}
