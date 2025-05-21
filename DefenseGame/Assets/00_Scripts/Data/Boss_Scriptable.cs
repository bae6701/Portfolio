using Data;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Boss_Scriptable", menuName = "Scriptable Objects/Boss_Scriptable")]
public class Boss_Scriptable : ScriptableObject, ILoader<int, MonsterData>
{
    public List<MonsterData> bossDatas = new List<MonsterData>();

    public Dictionary<int, MonsterData> MakeDict()
    {
        Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
        for (int i = 0; i < bossDatas.Count; i++)
        {
            //MonsterData newData = new MonsterData();
            //newData.CopyFrom(bossDatas[i]);
            dict.Add(i, bossDatas[i]);
        }
        return dict;
    }
}
