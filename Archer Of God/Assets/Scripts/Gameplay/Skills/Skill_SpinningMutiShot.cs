using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spinning Muti Shot", menuName = "Skills/SpinningMutiShot")]
public class Skill_SpinningMutiShot : Skill
{
    [Header("회전 연사 설정")]
    public float jumpForce = 10f;
    public string animationTriggerName = "jumpSpin";

    public override void Activate(Character caster)
    {
        if (caster == null) return;

        // 캐릭터를 점프시킵니다.
        caster.ApplyVerticalImpulse(jumpForce);

        // 애니메이션을 재생시킵니다.
        caster.TriggerAnimation(animationTriggerName);
    }

    public override Action GetAttackAction(Character caster)
    {
        return () =>
        {
            caster.FireProjectile(projectilePrefab);
        };
    }
}
