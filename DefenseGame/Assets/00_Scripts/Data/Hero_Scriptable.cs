using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroData
{
    public string name;
    public string description;
    public double attack;
    public float attackRange;
    public float attackSpeed;
    public float currentattackSpeed;
    public HeroType heroType;
    public HeroAttackType heroAttackType;
    public Rarity rarity;
    public RuntimeAnimatorController animator;
    public DebuffData debuffData;
    public SkillData skillData;
}

[CreateAssetMenu(fileName = "Hero_Scriptable", menuName = "Scriptable Objects/Hero_Scriptable")]
public class Hero_Scriptable : ScriptableObject, ILoader<HeroType, HeroData>
{
    public List<HeroData> heroDatas = new List<HeroData>();

    public Dictionary<HeroType, HeroData> MakeDict()
    {
        Dictionary<HeroType, HeroData> dict = new Dictionary<HeroType, HeroData>();
        for (int i = 0; i < heroDatas.Count; i++)
        {
            dict.Add(heroDatas[i].heroType, heroDatas[i]);
        }
        return dict;
    }
}
