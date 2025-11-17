using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using DG.Tweening;
using Unity.VisualScripting;
using System;

/// <summary>
/// [2주차 v2.7 - 최종본]
/// (v2.5 아키텍처 + v2.7 팝업/씬(Scene) UI 시스템)
/// (MonoBehaviour - Managers.cs가 생성)
/// </summary>
public class UIManager : MonoBehaviour // (v2.6) MonoBehaviour 유지
{
    // --- 1. UI 상태 ---
    int _order = 10; // 팝업 순서
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    
    // 씬(Scene) UI 참조 
    public UI_SceneRoot SceneUI { get; private set; }
    private Dictionary<UIPopupID, GameObject> _popupPrefabs = new Dictionary<UIPopupID, GameObject>();
    
    public GameObject Root
    {
        get
        {
            // (v2.7) 씬(Scene)에 @UI_Root가 있다는 것을 전제
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                Debug.LogError("@UI_Root is missing in MainGameScene!");
                root = new GameObject { name = "@UI_Root" };
            }
            return root;
        }
    }

    public void Init() 
    {
        // [신규 v2.21] 팝업 DB를 딕셔너리로 캐싱
        UIPrefabDatabase popupDB = Managers.Instance.PopupDB;
        if (popupDB != null)
        {
            _popupPrefabs.Clear();
            foreach (var entry in popupDB.popups)
            {
                _popupPrefabs.Add(entry.popupID, entry.prefab);
            }
        }
    }

    // --- 4. 씬(Scene) UI 등록 API (v2.7) ---
    // (UI_SceneRoot.cs (v2.7)]가 Start()에서 호출)
    public void RegisterSceneUI(UI_SceneRoot sceneUI) 
    { 
        SceneUI = sceneUI; 
        
        //UI 리스너 연결
        SceneUI.retryButton.onClick.AddListener(OnRetryButtonPressed);
        SceneUI.homeButton.onClick.AddListener(OnHomeButtonPressed);
        SceneUI.continueButton.onClick.AddListener(OnContinueButtonPressed);
        SceneUI.startButton.onClick.AddListener(OnStartButtonPressed);

        ShowTitlePanel();
        
        if (Managers.Instance.Game != null)
        {
            Managers.Instance.Game.OnBackgroundChanged += SetBackground;
        }
        
        Managers.Instance.Data.OnSceneLoaded();

        GameData data = Managers.Instance.Data.GameData;
        if (Managers.Instance.BackgroundDB != null && data.currentBackgroundSkinIndex < Managers.Instance.BackgroundDB.backgrounds.Count)
        {
            Sprite currentBg = Managers.Instance.BackgroundDB.backgrounds[data.currentBackgroundSkinIndex].backgroundSprite;
            SetBackground(currentBg);
        }
    }

    void OnDestroy()
    {
        if (Managers.Instance != null && Managers.Instance.Game != null)
        {
            Managers.Instance.Game.OnBackgroundChanged -= SetBackground;
        }
    }
    public void SetBackground(BackgroundEntry entry)
    {
        if (entry == null) return;
        SetBackground(entry.backgroundSprite);
    }

    public void SetBackground(Sprite bgSprite)
    {
        if (SceneUI != null && SceneUI.inGameBackgroundImage != null && bgSprite != null)
        {
            SceneUI.inGameBackgroundImage.sprite = bgSprite;
        }
    }

    // --- 5. v2.5 팝업 시스템 (핵심) ---
    public UI_Popup ShowPopupUI(UIPopupID id)
    {
        if (id == null || !_popupPrefabs.TryGetValue(id, out GameObject prefab))
        {
            Debug.LogError($"[UIManager] PopupID '{id?.name}'에(에) 등록된 Prefab이 없습니다!");
            return null;
        }

        GameObject go = Instantiate(prefab, Root.transform); // (Resource.Load 대신 Instantiate)
        UI_Popup popup = go.GetOrAddComponent<UI_Popup>(); // (UI_Popup 또는 그 자식)
        _popupStack.Push(popup);
        
        // (RectTransform 리셋)
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.transform.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;

        popup.Init(); 
		return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
		if (_popupStack.Count == 0) return;
        if (_popupStack.Peek() != popup)
        {
            Debug.LogError("Close Popup Failed!");
            return;
        }
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0) return;
        UI_Popup popup = _popupStack.Pop();

        Destroy(popup.gameObject); 
        
        popup = null;
        _order--;
    }

    // --- 6. 캔버스 설정 (v2.5) ---
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        go.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    // --- 7. UI 업데이트 (v2.7) ---
    public void UpdateInGameUI(int score, int combo, float time)
    {
        if (SceneUI == null) return;
        if (SceneUI.scoreText != null) SceneUI.scoreText.text = score.ToString();
        if (SceneUI.comboText != null) SceneUI.comboText.text = (combo > 1) ? $"{combo} COMBO" : "";
        if (SceneUI.timeText != null) SceneUI.timeText.text = time.ToString("F2");
    }
    public void UpdateCoinText(int totalCoins)
    {
        if (SceneUI != null && SceneUI.inGameCoinText != null) 
            SceneUI.inGameCoinText.text = totalCoins.ToString();
    }

    // --- 8. 패널 관리 (v2.7) ---
    public void StartGameUI()
    {
        if (SceneUI == null) return;
        SceneUI.titlePanel.SetActive(false);
        SceneUI.resultPanel.SetActive(false);
        SceneUI.inGameUIPanel.SetActive(true);
        
        UpdateInGameUI(0, 0, 0f);
        UpdateCoinText(Managers.Instance.Store.GetTotalCoins()); 
    }

    public void ShowTitlePanel()
    {
        if (SceneUI == null) return;
        SceneUI.titlePanel.SetActive(true);
        SceneUI.resultPanel.SetActive(false);
        SceneUI.inGameUIPanel.SetActive(false);
        Managers.Instance.Sound.Play(Define.SoundID.BGM_Lobby, Define.Sound.Bgm); 

        GameManager gameManager = Managers.Instance.Game;
        Managers.Instance.Store.EquipSkin(gameManager.currentSkinIndex); 
    }

    public void ShowResultPanel(int score, float time, int highScore, float bestTime, bool canContinue) 
    {
        if (SceneUI == null) return;
        SceneUI.inGameUIPanel.SetActive(false);
        SceneUI.resultPanel.SetActive(true);
        SceneUI.resultScoreText.text = $"Score: {score}";
        SceneUI.resultTimeText.text = $"Time: {time:F2}";
        SceneUI.resultHighScoreText.text = $"Best Score: {highScore}";
        SceneUI.resultBestTimeText.text = (bestTime == float.MaxValue) ? "Best Time: ---" : $"Best Time: {bestTime:F2}";
        
        bool adLoaded = Managers.Instance.Ads.CheckRewardedAdLoaded(); 
        SceneUI.continueButton.gameObject.SetActive(canContinue && adLoaded);
        SceneUI.continueButton.interactable = true;
    }
    // --- 9. 버튼 콜백 (v2.7) ---
    private void OnStartButtonPressed()
    {
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect); 
        SceneUI.startButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        Managers.Instance.Game.StartGame(); 
    }
    private void OnRetryButtonPressed()
    {
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
        SceneUI.retryButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        
        Managers.Instance.Game.StartGame();
    }
    private void OnHomeButtonPressed()
    {
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
        SceneUI.homeButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        ShowTitlePanel();
    }

    private void OnContinueButtonPressed()
    {
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
        SceneUI.continueButton.interactable = false; 
        Managers.Instance.Ads.ShowRewardedAd(); 
    }
    // --- 10. Juice API (v2.7) ---
    public void PunchScore()
    {
        if (SceneUI != null && SceneUI.scoreText != null)
        {
            SceneUI.scoreText.transform.DOKill(); 
            SceneUI.scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.5f);
        }
    }
    public void PunchCombo()
    {
        if (SceneUI != null && SceneUI.comboText != null)
        {
            SceneUI.comboText.transform.DOKill();
            SceneUI.comboText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 1, 0.5f);
        }
    }
    
    // --- 11. (v2.5) Clear ---
    public void Clear()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
        SceneUI = null;
    }
}