using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [2주차 v2.7] 배경 스킨 데이터베이스 (ScriptableObject)
/// </summary>
[System.Serializable]
public class BackgroundEntry // (배경 스킨 데이터)
{
    public Define.BackgroundID skinID;
    public string displayName; // [신규 v2.12] 상점에 표시될 이름
    public int price;
    public Sprite backgroundSprite;
}

[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "ShapeFitRush/Background Database")]
public class BackgroundDatabase : ScriptableObject
{
    public List<BackgroundEntry> backgrounds;
}