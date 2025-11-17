using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// [3주차 v2.8] (지적 4 해결)
/// 모든 게임 데이터(GameData)의 저장(Save)/로드(Load)를 전담.
/// (MonoBehaviour - Managers.cs가 생성)
/// </summary>
public class DataManager : MonoBehaviour
{
    public GameData GameData { get; private set; }
    private const string GAME_DATA_KEY = "GameData";
    private bool _hasCheckedDailyReward = false; 

    public void Init()
    {
        LoadGame();      
    }

    public void OnSceneLoaded()
    {
        CheckDailyReward();
    }

    /// <summary>
    /// 게임 시작 시 PlayerPrefs에서 Json 데이터를 로드
    /// </summary>
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(GAME_DATA_KEY))
        {
            string json = PlayerPrefs.GetString(GAME_DATA_KEY);
            GameData = JsonUtility.FromJson<GameData>(json);
            if (GameData == null)
            {
                Debug.LogWarning("GameData 로드 실패. 새 데이터를 생성합니다.");
                GameData = new GameData();
            }
        }
        else
        {
            Debug.Log("저장된 데이터 없음. 새 데이터를 생성합니다.");
            GameData = new GameData();
        }
    }

    /// <summary>
    /// 현재 GameData 객체를 Json으로 변환하여 PlayerPrefs에 저장
    /// </summary>
    public void SaveGame()
    {
        if (GameData == null)
        {
            Debug.LogError("GameData가 null입니다. 저장을 중단합니다.");
            return;
        }

        string json = JsonUtility.ToJson(GameData);
        PlayerPrefs.SetString(GAME_DATA_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("--- Game Data Saved ---");
    }
    
    private DateTime GetLastLoginTime()
    {
        try { return DateTime.Parse(GameData.lastLoginTime); }
        catch { return DateTime.MinValue; }
    }

    private void CheckDailyReward()
    {
        if (_hasCheckedDailyReward) return; 
        _hasCheckedDailyReward = true;
    }

    /// <summary>
    /// UIManager가 호출: "보상 받기" 버튼 활성화 여부
    /// </summary>
    public bool IsDailyRewardReady()
    {
        DateTime now = DateTime.Now;
        DateTime lastLogin = GetLastLoginTime();
        
        return (now - lastLogin).TotalHours >= 24;
    }
    
    /// <summary>
    /// UIManager가 호출: "남은 시간" 표시 (예: 23:59:59)
    /// </summary>
    public TimeSpan GetDailyRewardTimeRemaining()
    {
        if (IsDailyRewardReady())
            return TimeSpan.Zero;
            
        DateTime now = DateTime.Now;
        DateTime nextRewardTime = GetLastLoginTime().AddHours(24); 
        
        return nextRewardTime - now;
    }

    /// <summary>
    /// UIManager가 호출: "보상 받기" 버튼 클릭 시
    /// </summary>
    public int ClaimDailyReward()
    {
        if (!IsDailyRewardReady())
            return 0; // (보상 받을 수 없음)
            
        Debug.Log("일일 보상 지급!");
        int rewardAmount = 50; 
        Managers.Instance.Store.AddCoins(rewardAmount);
        GameData.lastLoginTime = DateTime.Now.ToString();
        SaveGame();
        
        return rewardAmount;
    }
}