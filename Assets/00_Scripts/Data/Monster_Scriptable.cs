using Data;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Monster_Scriptable", menuName = "Scriptable Objects/Monster_Scriptable")]
public class Monster_Scriptable : ScriptableObject, ILoader<int, MonsterData>
{
    public List<MonsterData> monsterDatas = new List<MonsterData>();

    public Dictionary<int, MonsterData> MakeDict()
    {
        Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
        for (int i = 0; i < monsterDatas.Count; i++)
        {
            //MonsterData newData = new MonsterData();
            //newData.CopyFrom(monsterDatas[i]);
            dict.Add(i, monsterDatas[i]);
        }           
        return dict;
    }
}
