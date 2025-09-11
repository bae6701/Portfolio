using System;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [Header("스킬 정보")]
    public string skillName;
    [TextArea] public string description;
    public Sprite skillIcon;
    public float cooldown = 5f;
    public GameObject vfxPrefab;

    [Header("스킬 동작 설정")]
    public GameObject projectilePrefab;

    public abstract void Activate(Character caster);

    public abstract Action GetAttackAction(Character caster);
}
