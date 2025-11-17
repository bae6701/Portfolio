using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [3주차 v2.9] (지적 5 해결: 하드코딩)
/// SoundID와 AudioClip을 매핑하는 데이터베이스 (ScriptableObject)
/// </summary>
[System.Serializable]
public class SoundEntry
{
    [Tooltip("GameManager 등에서 호출할 ID (예: SFX_Tap, BGM_Lobby)")]
    public Define.SoundID soundID;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "ShapeFitRush/Sound Database")]
public class SoundDatabase : ScriptableObject
{
    public List<SoundEntry> sounds;
}