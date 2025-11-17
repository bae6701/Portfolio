using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkinEntry
{
    public Define.SkinID skinID;
    public string displayName;
    public int price;
    
    [Header("Game Assets")]
    public GameObject blockPrefab; 
    public Sprite frameSprite;   // 
    
    // (선택적) 상점 UI에 표시할 아이콘 등 추가 가능
    // public Sprite storeIcon; 
}
/// <summary>
/// (원칙: ScriptableObject) 모든 스킨 에셋과 가격을 통합 관리하는 '데이터베이스' 에셋.
/// (지적 3: 자동화) 상점과 스포너가 이 파일 하나만 참조하게 됨.
/// </summary>
[CreateAssetMenu(fileName = "SkinDatabase", menuName = "ShapeFitRush/Skin Database")]
public class SkinDatabase : ScriptableObject
{
    public List<SkinEntry> skins;
}