using Data;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveSkillData
{
    public int openLevel;   
    [TextArea(2, 5)]
    public string description;
}

[System.Serializable]
public class HeroPassiveSet
{
    public HeroType heroType;

    [Tooltip("패시브는 5, 15, 30레벨 기준으로 오픈됩니다.")]
    public List<PassiveSkillData> passives = new List<PassiveSkillData>();
}

[CreateAssetMenu(fileName = "PassiveSkill_Scriptable", menuName = "Scriptable Objects/PassiveSkill_Scriptable")]
public class PassiveSkill_Scriptable : ScriptableObject, ILoader<HeroType, HeroPassiveSet>
{
    public List<HeroPassiveSet> heroPassiveList = new List<HeroPassiveSet>();

    public Dictionary<HeroType, HeroPassiveSet> MakeDict()
    {
        Dictionary<HeroType, HeroPassiveSet> dict = new Dictionary<HeroType, HeroPassiveSet>();
        for (int i = 0; i < heroPassiveList.Count; i++)
        {
            dict.Add(heroPassiveList[i].heroType, heroPassiveList[i]);
        }
        return dict;
    }
}
