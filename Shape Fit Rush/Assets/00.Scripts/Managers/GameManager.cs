using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// (리빌드 v2.5 최종본 - 버그 수정)
/// (v2.5: 유저님의 Managers.cs 아키텍처와 호환)
/// (v2.5: 순수 C# 클래스로 변경, Managers.cs가 생성)
/// (v2.5: Managers.Instance.Settings로 호출 수정)
/// </summary>
public class GameManager : MonoBehaviour
{   
    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action<SkinEntry> OnSkinChanged;
    public event Action<BackgroundEntry> OnBackgroundChanged;
    private GameData _data { get { return Managers.Instance.Data.GameData; } }

    public int currentSkinIndex => _data.currentSkinIndex;
    public int currentBadBlockSkinIndex => _data.currentBadBlockSkinIndex;
    public int currentBackgroundSkinIndex => _data.currentBackgroundSkinIndex;

    private int highScore { get => _data.highScore; set => _data.highScore = value; }
    private float bestTime { get => _data.bestTime; set => _data.bestTime = value; }

    // --- 1. 상태 변수 (동일) ---
    public bool IsGameOver { get; private set; } = true;
    private int score = 0, combo = 0;
    private float totalPlayTime = 0f;
    public bool isFeverMode { get; private set; } = false;
    private bool hasContinued = false;
    private static int gameOverCount = 0;
    private GameObject currentFeverAura;
    private Coroutine _feverCoroutine;
    private Coroutine _gameOverCoroutine;
    private Coroutine _updateTimerCoroutine;

    private WaitForSeconds _delayFever;
    private WaitForSeconds _delayShowResult = new WaitForSeconds(0.75f);
    private WaitForSeconds _delayUpdateTimer = new WaitForSeconds(0.1f);

    
    // --- 3. 난이도 (동일) ---
    public float CurrentBlockSpeed { get; private set; }
    public float CurrentBadBlockChance { get; private set; }
    public float CurrentSpawnInterval { get; private set; }

    // --- 5. 핵심 로직 (v2.5) ---
    public void Init()
    {
        if (Managers.Instance.Ads != null)
        {
            Managers.Instance.Ads.OnRewardedAdCompleted += ContinueGame;
        }

        if (Managers.Instance.Settings != null)
        {
            _delayFever = new WaitForSeconds(Managers.Instance.Settings.feverDuration);
        }
        else
        {
            Debug.LogError("GameSettings가(이) null이라서 피버 시간을(을) 캐시할 수 없습니다!");
            _delayFever = new WaitForSeconds(5f); // (기본값)
        }
    }
    
    void Update()
    {
        if (!IsGameOver)
        {
            totalPlayTime += Time.deltaTime;       
        }
    }
    
    void OnDestroy()
    {
        if (Managers.Instance != null && Managers.Instance.Ads != null) {
             Managers.Instance.Ads.OnRewardedAdCompleted -= ContinueGame;
        }
    }
    
    private IEnumerator Co_UpdateTimerUI()
    {
        while (!IsGameOver)
        {
            Managers.Instance.UI.UpdateInGameUI(score, combo, totalPlayTime);
            yield return _delayUpdateTimer; // (캐시된 0.1초 대기)
        }
    }
    // --- 7. 판정 결과 (v2.5) ---
    public void TriggerPerfect()
    {
        score += 1;
        combo++;
        
        if (score > 0 && score % Managers.Instance.Settings.scorePerCoin == 0)
        {
            Managers.Instance.Store.AddCoins(1);
        }
        
        UpdateDifficulty(); 
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Perfect, Define.Sound.Effect); 
        Managers.Instance.Judgment.PopVFX(Define.SoundID.SFX_Perfect);

        if (!isFeverMode) 
        {
            Managers.Instance.UI.PunchScore(); 
            Managers.Instance.UI.PunchCombo(); 
            if (combo == 10) Managers.Instance.Sound.Play(Define.SoundID.Voice_Combo10, Define.Sound.Effect);
            if (combo == 30) Managers.Instance.Sound.Play(Define.SoundID.Voice_Combo30, Define.Sound.Effect);
            if (combo > 0 && combo % Managers.Instance.Settings.feverComboThreshold == 0) { StartFeverMode(); }
        }

        Managers.Instance.UI.UpdateInGameUI(score, combo, totalPlayTime);
    }

    public void TriggerGood()
    {
        combo = 0; 
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Good, Define.Sound.Effect);
        Managers.Instance.Judgment.PopVFX(Define.SoundID.SFX_Good);
        Managers.Instance.UI.UpdateInGameUI(score, combo, totalPlayTime);
    }
    
    public void TriggerFeverPerfect(Block blockToPush)
    {
        score += 1;
        combo++;
        if (score > 0 && score % Managers.Instance.Settings.scorePerCoin == 0) {
            Managers.Instance.Store.AddCoins(1);
        }
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Perfect, Define.Sound.Effect);
        Managers.Instance.Judgment.PopVFX(Define.SoundID.SFX_Perfect);
        
        Managers.Instance.UI.PunchScore(); 
        Managers.Instance.UI.PunchCombo(); 
        
        Managers.Instance.Pool.Push(blockToPush.GetComponent<Poolable>());
        Managers.Instance.UI.UpdateInGameUI(score, combo, totalPlayTime);
    }

    public void TriggerMiss()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        combo = 0;
        
        OnGameOver?.Invoke(); // "게임 오버!" 방송

        if (_feverCoroutine != null) StopCoroutine(_feverCoroutine);
        if (_updateTimerCoroutine != null) StopCoroutine(_updateTimerCoroutine);       
        if (isFeverMode) StopFeverMode(false);

        Managers.Instance.Judgment.PopVFX(Define.SoundID.SFX_Miss);
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Miss, Define.Sound.Effect);
        Managers.Instance.UI.UpdateInGameUI(score, combo, totalPlayTime);

        if (_gameOverCoroutine != null) StopCoroutine(_gameOverCoroutine);
        _gameOverCoroutine = StartCoroutine(Co_ShowResultPanelAfterDelay());
    }

    private IEnumerator Co_ShowResultPanelAfterDelay()
    {
        yield return _delayShowResult; // (캐시된 0.75초 대기)
        if (!hasContinued)
        {
            hasContinued = true; 
            Managers.Instance.UI.ShowResultPanel(score, totalPlayTime, highScore, bestTime, canContinue: true); 
        }
        else
        {
            FinalGameOver();    
        }
    }
    private void FinalGameOver()
    {
        gameOverCount++;
        if (gameOverCount % 3 == 0)
        {
            Managers.Instance.Ads.ShowInterstitialAd(); 
        }
        Managers.Instance.Sound.Play(Define.SoundID.Voice_GameOver, Define.Sound.Effect);
        
        bool isNewHighScore = false;
        if (score > highScore) { 
            highScore = score; 
            bestTime = totalPlayTime;
            isNewHighScore = true;
        }
        else if (score == highScore && totalPlayTime < bestTime) { 
            bestTime = totalPlayTime; 
        }
        Managers.Instance.Data.SaveGame();
        
        if (isNewHighScore) // (최고 점수일 때만 제출)
        {
            // Managers.Instance.Social.SubmitScore(highScore);
        }

        Managers.Instance.UI.ShowResultPanel(score, totalPlayTime, highScore, bestTime, canContinue: false);
    }

    private void ContinueGame()
    {
        Debug.Log("Game Continued (Reward Received)!");
        IsGameOver = false;
        Managers.Instance.UI.StartGameUI(); 
        Managers.Instance.Sound.Play(Define.SoundID.BGM_InGame, Define.Sound.Bgm);
        
        OnGameStart?.Invoke();

        if (_updateTimerCoroutine != null) StopCoroutine(_updateTimerCoroutine);
        _updateTimerCoroutine = StartCoroutine(Co_UpdateTimerUI());

        Managers.Instance.UI.UpdateInGameUI(score, combo, totalPlayTime);
    }

    // --- 9. 게임 시작/재시작 ---
    public void StartGame()
    {
        if (_gameOverCoroutine != null) StopCoroutine(_gameOverCoroutine);
        if (_feverCoroutine != null) StopCoroutine(_feverCoroutine);
        if (_updateTimerCoroutine != null) StopCoroutine(_updateTimerCoroutine);
        _gameOverCoroutine = null;
        _feverCoroutine = null;
        _updateTimerCoroutine = null;

        IsGameOver = false;
        score = 0;
        combo = 0;
        totalPlayTime = 0f;
        isFeverMode = false;
        hasContinued = false;        
        
        ResetDifficulty(); 
        Managers.Instance.UI.StartGameUI(); 
        Managers.Instance.Sound.Play(Define.SoundID.BGM_InGame, Define.Sound.Bgm);

        OnGameStart?.Invoke();

        _updateTimerCoroutine = StartCoroutine(Co_UpdateTimerUI());

        Managers.Instance.UI.UpdateInGameUI(score, combo, totalPlayTime);
    }

    // --- 10. 피버 모드 로직 ---
    private void StartFeverMode()
    {
        if (isFeverMode) return; 
        isFeverMode = true;
        Managers.Instance.Sound.Play(Define.SoundID.Voice_FeverTime, Define.Sound.Effect);
        Managers.Instance.Sound.Play(Define.SoundID.BGM_Fever, Define.Sound.Bgm);

        currentFeverAura = Managers.Instance.Judgment.PopFeverAura();
        
        _feverCoroutine = StartCoroutine(Co_FeverTimer());
    }

    IEnumerator Co_FeverTimer()
    {
        yield return _delayFever; // (캐시된 피버 시간 대기)
        StopFeverMode();
    }

    private void StopFeverMode(bool playBGM = true)
    {
        if (!isFeverMode) return;
        isFeverMode = false;
        
        if (_feverCoroutine != null) { StopCoroutine(_feverCoroutine); _feverCoroutine = null; }

        if (playBGM)
        {
            Managers.Instance.Sound.Play(Define.SoundID.BGM_InGame, Define.Sound.Bgm);
        }
        
        if (currentFeverAura != null) { 
            Managers.Instance.Pool.Push(currentFeverAura.GetComponent<Poolable>()); 
            currentFeverAura = null; 
        }
    }

    // --- 11. 핵심 로직 (Save/Load/Difficulty) ---
    private void UpdateDifficulty()
    {
        float difficultyValue = Managers.Instance.Settings.difficultyCurve.Evaluate(score);
        CurrentBlockSpeed = Mathf.Lerp(Managers.Instance.Settings.minBlockSpeed, Managers.Instance.Settings.maxBlockSpeed, difficultyValue);
        CurrentBadBlockChance = Mathf.Lerp(Managers.Instance.Settings.minBadBlockChance, Managers.Instance.Settings.maxBadBlockChance, difficultyValue);
        CurrentSpawnInterval = Mathf.Lerp(Managers.Instance.Settings.minSpawnInterval, Managers.Instance.Settings.maxSpawnInterval, difficultyValue);
    }
    private void ResetDifficulty()
    {
        if (Managers.Instance.Settings == null)
        {
            Debug.LogError("!!! GameManager.ResetDifficulty: Managers.Instance.Settings가 null입니다! Bootstrapper가 로드에 실패했습니다.");
            return;
        }

        CurrentBlockSpeed = Managers.Instance.Settings.minBlockSpeed;
        CurrentBadBlockChance = Managers.Instance.Settings.minBadBlockChance;
        CurrentSpawnInterval = Managers.Instance.Settings.minSpawnInterval;
    }
    
    public void OnSkinEquipped()
    {
        if (Managers.Instance.SkinDB != null)
            OnSkinChanged?.Invoke(Managers.Instance.SkinDB.skins[currentSkinIndex]);
    }
    
    // (StoreManager가 EquipBackgroundSkin() 성공 시 호출)
    public void OnBackgroundEquipped()
    {
        if (Managers.Instance.BackgroundDB != null)
            OnBackgroundChanged?.Invoke(Managers.Instance.BackgroundDB.backgrounds[currentBackgroundSkinIndex]);
    }
}