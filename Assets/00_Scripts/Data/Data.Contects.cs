using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class StatInfo
    {
        public double hp;
        public double maxHp;
        public float speed;
        public float currentSpeed;
        public float defence;
        public int gold;
    }

    [System.Serializable]
    public class MonsterData
    {
        public MonsterType monsterType;
        public string monsterName;
        public string prefabPath;

        public StatInfo statInfo;
    }

    [System.Serializable]
    public class SkillData
    {
        public SkillType skillType;
        public string name;
        public string description;

        public float coolTime;
        public float duration;

        public double skillDamage;
        public float skillRange;
    }

    [System.Serializable]
    public class DebuffData
    {
        public float slowAmount;
        public float slowduration;
        public float stunduration;
    }
   

    public enum Rarity
    { 
        COMMON,
        UNCOMMON, 
        RARE, 
        HERO,
        LEGENDARY, 
    }

    public enum HeroType
    {
        NONE,
        ARCHER,
        BANDIT,
        CLERIC,
        WARRIOR,
        WIZARD,
        GUN,        
        SHIELD,           
        HAMMER,
        SWORD,
        WITCH,
    }

    public enum MonsterType
    { 
        NONE,
        MOB,
        BOSS,
    }

    public enum LobbyButtons
    {
        Hero,
        Guild,
        Battle,
        Shop,
    }

    public enum DebuffType
    { 
        SLOW,
        STUN,
    }

    public enum HeroAttackType
    { 
        MELEE,
        LONGRANGE,
    }

    public enum HostType
    { 
        HOST,
        CLIENT,
        ALL,
    }

    public enum SkillType
    { 
        NONE,
        GUN,
        SHIELD,
    }
    public enum CharacterState
    {
        IDLE,
        ATTACK,
        MOVE,
        SKILL,
        DEAD,
    }  
}

