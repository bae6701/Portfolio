using UnityEngine;

public class IceProjectile : Projectile
{
    [Header("빙결 효과 설정")]
    [SerializeField] private float freezeDuration = 2f; // 얼리는 시간
    [SerializeField] private GameObject freezeVfxPrefab; // 얼음 이펙트 프리팹

    protected override void ApplyEffect(Character target)
    {
        base.ApplyEffect(target);

        if (target != null)
        {
            target.Freeze(freezeDuration, freezeVfxPrefab).Forget();
        }
    }
}
