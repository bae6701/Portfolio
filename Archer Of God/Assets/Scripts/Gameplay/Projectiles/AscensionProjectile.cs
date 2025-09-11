using UnityEngine;

public class AscensionProjectile : Projectile
{
    [Header("상승 효과 설정")]
    [Tooltip("적을 띄우는 힘의 크기")]
    [SerializeField] private float launchPower = 25f;

    protected override void ApplyEffect(Character target)
    {
        if (target != null)
        {
            target.ApplyVerticalImpulse(launchPower);
        }
    }
}
