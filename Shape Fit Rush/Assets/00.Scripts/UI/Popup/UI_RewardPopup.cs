using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections; 

/// <summary>
///  '보상 받기' 또는 '남은 시간'을(를) 표시하며 타이머를 돌림)
/// </summary>
public class UI_RewardPopup : UI_Popup
{
    // --- 인스펙터 연결 (프리팹 내부) ---
    [Header("UI Components")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text rewardAmountText;
    [SerializeField] private TMP_Text timeRemainingText; // [신규] "남은 시간: 23:59:59"
    [SerializeField] private Button claimButton; // [신규] "보상 받기" 버튼
    [SerializeField] private Button closeButton;

    [Header("Config")]
    [SerializeField] private int rewardAmount = 50;
    
    private Coroutine _timerCoroutine;
    private DataManager _dataManager;

    public override void Init()
    {
        base.Init(); // (SetCanvas, GraphicRaycaster 호출)
        
        _dataManager = Managers.Instance.Data;
        
        // 1. 리스너 연결
        closeButton.onClick.AddListener(OnClosePressed);
        claimButton.onClick.AddListener(OnClaimButtonPressed);
        
        // 2. 팝업이 열릴 때 즉시 UI 갱신
        UpdateVisuals();
    }

    private void OnClaimButtonPressed()
    {
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Coin, Define.Sound.Effect);
        
        // 1. 데이터 매니저에게 보상 요청
        int claimedAmount = _dataManager.ClaimDailyReward();
        
        if (claimedAmount > 0)
        {
            Managers.Instance.UI.UpdateCoinText(Managers.Instance.Store.GetTotalCoins());
        }
        
        // 2. UI를 '남은 시간' 상태로 즉시 갱신
        UpdateVisuals();
    }

    private void OnClosePressed()
    {
        // (v2.26) 팝업이 닫힐 때 타이머가 돌고 있다면 중지
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
        
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
        Managers.Instance.UI.ClosePopupUI(this); 
    }

    /// <summary>
    /// [신규 v2.26] 팝업의 현재 상태를 갱신
    /// </summary>
    private void UpdateVisuals()
    {
        if (_dataManager.IsDailyRewardReady())
        {
            // 1. 보상 받을 수 있음
            titleText.text = "일일 보상";
            rewardAmountText.text = $"+{rewardAmount} 코인";
            
            timeRemainingText.gameObject.SetActive(false);
            claimButton.gameObject.SetActive(true);
            claimButton.interactable = true;
            
            // (보상을 받을 수 있으므로, 타이머가 돌고 있다면 중지)
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }
        else
        {
            // 2. 남은 시간이 있음
            titleText.text = "다음 보상까지";
            rewardAmountText.text = ""; // (보상 텍스트 숨김)
            
            timeRemainingText.gameObject.SetActive(true);
            claimButton.gameObject.SetActive(false);
            claimButton.interactable = false;
            
            // [핵심] 타이머가 아직 실행 중이 아니라면, 새로 시작
            if (_timerCoroutine == null)
            {
                _timerCoroutine = StartCoroutine(Co_UpdateTimer());
            }
        }
    }

    /// <summary>
    /// [신규 v2.26] 남은 시간을 1초마다 갱신하는 타이머
    /// </summary>
    private IEnumerator Co_UpdateTimer()
    {
        while (!_dataManager.IsDailyRewardReady())
        {
            TimeSpan remaining = _dataManager.GetDailyRewardTimeRemaining();
            timeRemainingText.text = $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            
            yield return new WaitForSeconds(1.0f); // 1초마다 갱신
        }
        
        // (while문 종료 = 보상 받을 시간 도달)
        
        // 1. 마지막으로 UI를 "보상 받기"로 갱신
        UpdateVisuals();
        
        // 2. 코루틴 스스로 종료
        _timerCoroutine = null;
    }
}