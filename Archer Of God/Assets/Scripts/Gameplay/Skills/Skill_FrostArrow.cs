using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New FrostArrow", menuName = "Skills/FrostArrow")]
public class Skill_FrostArrow : Skill
{
    [Header("빙결 화살 설정")]
    public string animationTriggerName = "FrostArrowShot";

    public override void Activate(Character caster)
    {
        if (caster == null) return;
        caster.TriggerAnimation(animationTriggerName);
    }
    public override Action GetAttackAction(Character caster)
    {
        return ()=> 
        {
            caster.FireProjectile(projectilePrefab);
        };
    }
}
