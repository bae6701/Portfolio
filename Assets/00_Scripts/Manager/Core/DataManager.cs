using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{   
    public Dictionary<HeroType, HeroData> heroDict = new Dictionary<HeroType, HeroData>();
    public Dictionary<int, MonsterData> monsterDict = new Dictionary<int, MonsterData>();
    public Dictionary<int, MonsterData> bossDict = new Dictionary<int, MonsterData>();
    public Dictionary<int, CombineData> combineDict = new Dictionary<int, CombineData>();
    public Dictionary<HeroType, HeroPassiveSet> heroPassiveDict = new Dictionary<HeroType, HeroPassiveSet>();
    public Dictionary<Rarity, float> settingDict = new Dictionary<Rarity, float>();
    public SpriteAtlas atlas;

    public Sprite GetAtlas(string name)
    { 
        return atlas.GetSprite(name);     
    }

    public void Init()
    {
        atlas = Resources.Load<SpriteAtlas>("Prefabs/Atlas/Atlas");
        heroDict = LoadScriptable<Hero_Scriptable, HeroType, HeroData>("Hero_Scriptable").MakeDict();
        monsterDict = LoadScriptable<Monster_Scriptable, int, MonsterData>("Monster_Scriptable").MakeDict();
        bossDict = LoadScriptable<Boss_Scriptable, int, MonsterData>("Boss_Scriptable").MakeDict();
        combineDict = LoadScriptable<Combine_Scriptable, int, CombineData>("Combine_Scriptable").MakeDict();
        settingDict = LoadScriptable<Setting_Scriptable, Rarity, float>("Setting_Scriptable").MakeDict();
        heroPassiveDict = LoadScriptable<PassiveSkill_Scriptable, HeroType, HeroPassiveSet>("PassiveSkill_Scriptable").MakeDict();
    }

    T LoadScriptable<T, Key, Value>(string path) where T : ScriptableObject, ILoader<Key, Value>
    {
        return Resources.Load<T>($"Prefabs/Character/Scriptable/{path}");
    }
}