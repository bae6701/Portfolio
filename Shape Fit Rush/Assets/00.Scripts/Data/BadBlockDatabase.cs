using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [2주차 v2.7] 방해 블록 스킨 데이터 (ScriptableObject)
/// </summary>
[System.Serializable]
public class BadBlockEntry // (방해 블록 스킨 데이터)
{
    public Define.BadBlockID skinID;
    public string displayName; 
    public int price;
    public Sprite badBlockSprite;
}

[CreateAssetMenu(fileName = "BadBlockDatabase", menuName = "ShapeFitRush/BadBlock Database")]
public class BadBlockDatabase : ScriptableObject
{
    public List<BadBlockEntry> badBlocks;
}