using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ascension", menuName = "Skills/Ascension")]
public class Skill_Ascension : Skill
{
    [Header("수직 강타 설정")]
    [Tooltip("화살이 생성될 높이")]
    public float spawnHeight = 9f;

    [Header("시전자 애니메이션")]
    [Tooltip("하늘을 조준하거나 신호를 보내는 애니메이션의 트리거 이름")]
    public string animationTriggerName = "ascension";

    public override void Activate(Character caster)
    {
        if (caster == null) return;
        caster.TriggerAnimation(animationTriggerName);
    }

    public override Action GetAttackAction(Character caster)
    {
        return () =>
        {
            if (projectilePrefab == null)
            {
                Debug.LogError("떨어뜨릴 화살 프리팹이 지정되지 않았습니다!");
                return;
            }

            Character target = caster.GetTarget();
            if (target == null)
            {
                Debug.LogWarning("수직 강타를 사용할 타겟을 찾을 수 없습니다.");
                return;
            }

            Vector3 spawnPosition = target.transform.position;
            spawnPosition.y += spawnHeight;

            Poolable poolable = PoolManager.Instance.Pop(projectilePrefab);
            GameObject arrow = poolable.gameObject;
            arrow.transform.position = spawnPosition;

            Projectile projectileComponent = arrow.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.Launch(target.transform, true);
            }
        };
    }
}
