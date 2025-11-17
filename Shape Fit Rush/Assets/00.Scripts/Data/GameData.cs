using System;

/// <summary>
/// [3주차 v2.8] (지적 4 해결)
/// PlayerPrefs에 'Json'으로 통째로 저장될 데이터 객체 (POCO)
/// (MonoBehaviour가 아님)
/// </summary>
[System.Serializable]
public class GameData
{
    // 1. 재화 및 점수
    public int totalCoins;
    public int highScore;
    public float bestTime;

    // 2. 스킨 (Bitmask)
    public int currentSkinIndex;
    public int unlockedSkinsBitmask;
    public int currentBadBlockSkinIndex;
    public int unlockedBadSkinsBitmask;
    public int currentBackgroundSkinIndex; // (3단계)
    public int unlockedBackgroundSkinsBitmask; // (3단계)

    // 3. 설정 (3주차 신규)
    public bool isBgmOn;
    public bool isSfxOn;

    // 4. 리텐션 (3주차 신규)
    public string lastLoginTime; // (DateTime을 string으로 저장)

    /// <summary>
    /// 게임 첫 실행 시 기본값 설정
    /// </summary>
    public GameData()
    {
        totalCoins = 0;
        highScore = 0;
        bestTime = float.MaxValue;
        
        currentSkinIndex = 0;
        unlockedSkinsBitmask = 1; // (0번 스킨은 기본 잠금 해제)
        currentBadBlockSkinIndex = 0;
        unlockedBadSkinsBitmask = 1; // (0번 스킨은 기본 잠금 해제)
        currentBackgroundSkinIndex = 0;
        unlockedBackgroundSkinsBitmask = 1;

        isBgmOn = true;
        isSfxOn = true;
        
        lastLoginTime = DateTime.MinValue.ToString();
    }
}