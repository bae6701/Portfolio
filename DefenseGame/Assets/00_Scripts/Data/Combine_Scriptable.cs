using Data;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombineData
{
    public HeroType SpawnheroType;
    public List<HeroType> SpawnMaterialsType;
}

[CreateAssetMenu(fileName = "Combine_Scriptable", menuName = "Scriptable Objects/Combine_Scriptable")]
public class Combine_Scriptable : ScriptableObject, ILoader<int, CombineData>
{
    public List<CombineData> combineDatas = new List<CombineData>();

    public Dictionary<int, CombineData> MakeDict()
    {
        Dictionary<int, CombineData> dict = new Dictionary<int, CombineData>();
        for (int i = 0; i < combineDatas.Count; i++)
        {
            dict.Add(i, combineDatas[i]);
        }
        return dict;
    }
}
