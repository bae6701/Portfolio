using UnityEngine;

/// <summary>
/// (원칙: ScriptableObject) 게임의 모든 밸런스 수치를 관리하는 '설정값' 에셋.
/// (지적 2: 기획자가 코드를 건드리지 않고 이 파일만 수정하여 밸런싱)
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "ShapeFitRush/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Judgment Config")]
    public float perfectThreshold = 0.5f; // 
    public float goodThreshold = 1.0f;    // 

    [Header("Difficulty Config")]
    [Tooltip("난이도 커브. X축=Score(Perfect 횟수), Y축=값(0~1)")]
    public AnimationCurve difficultyCurve;
    
    [Space(10)]
    [Tooltip("난이도 0일 때(Min)의 블록 속도")]
    public float minBlockSpeed = 5f;
    [Tooltip("난이도 1일 때(Max)의 블록 속도")]
    public float maxBlockSpeed = 15f;
    
    [Tooltip("난이도 0일 때(Min)의 방해 블록 확률 (0.0 ~ 1.0)")]
    [Range(0, 1)]
    public float minBadBlockChance = 0.1f;
    [Tooltip("난이도 1일 때(Max)의 방해 블록 확률")]
    [Range(0, 1)]
    public float maxBadBlockChance = 0.5f;

    [Tooltip("난이도 0일 때(Min)의 스폰 간격 (초)")]
    public float minSpawnInterval = 1.5f;
    [Tooltip("난이도 1일 때(Max)의 스폰 간격 (초)")]
    public float maxSpawnInterval = 0.5f;

    [Header("Fever Config")]
    public int feverComboThreshold = 30; // 
    public float feverDuration = 5f;     // 

    [Header("Economy Config")]
    [Tooltip("이 점수(Score)마다 1 코인을 획득")]
    public int scorePerCoin = 10; // [신규] '점수 비율' 코인 획득
}