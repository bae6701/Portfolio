using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIPrefabEntry
{
    public UIPopupID popupID; // (예: StorePopupID.asset)
    public GameObject prefab; // (예: StorePanel.prefab)
}

[CreateAssetMenu(fileName = "UIPrefabDatabase", menuName = "ShapeFitRush/UI Prefab Database")]
public class UIPrefabDatabase : ScriptableObject
{
    public List<UIPrefabEntry> popups;
}