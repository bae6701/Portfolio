using Data;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Setting_Scriptable", menuName = "Scriptable Objects/Setting_Scriptable")]
public class Setting_Scriptable : ScriptableObject, ILoader<Rarity, float>
{
    [Header("È®·ü")]
    [Range(0.0f, 100.0f)]
    public float[] Rarity_Percentage = new float[Enum.GetValues(typeof(Rarity)).Length];

    public Dictionary<Rarity, float> MakeDict()
    {
        Dictionary<Rarity, float> dict = new Dictionary<Rarity, float>();
        int index = 0;
        foreach(Rarity rarity in Enum.GetValues(typeof(Rarity)))
        {
            dict.Add(rarity, Rarity_Percentage[index]);
            index++;
        }
        return dict;
    }
}
