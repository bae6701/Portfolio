using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// [2주차 v2.7] (지적 3 해결)
/// 탭/그리드/프리팹을 관리하고 자동 생성합니다.
/// </summary>
public class UI_StorePopup : UI_Popup // (v2.7) UI_Popup 상속
{
    // --- 1. 인스펙터 연결 (프리팹 내부) ---
    [Header("Store Components")]
    [SerializeField] private TMP_Text storeCoinText;
    [SerializeField] private Button closeStoreButton;
    
    [Header("Tabs")]
    [SerializeField] private Button tabBlockSkin;
    [SerializeField] private Button tabBadBlockSkin;
    [SerializeField] private Button tabBackgroundSkin;

    [Header("Scroll Components")]
    [Tooltip("Viewport (ScrollRect, RectMask2D 컴포넌트가 있는 부모)")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("Item Panels (Grid Roots)")]
    [SerializeField] private RectTransform panelBlockSkin;
    [SerializeField] private RectTransform panelBadBlockSkin;
    [SerializeField] private RectTransform panelBackgroundSkin;

    [Header("Item Prefab")]
    [SerializeField] private GameObject storeItemPrefab; // (UI_StoreItem.cs가 부착된 프리팹)
    
    private GameManager _gameManager;
    private StoreManager _storeManager;
    private SoundManager _soundManager;
    private SkinDatabase _skinDB;
    private BadBlockDatabase _badBlockDB;
    private BackgroundDatabase _backgroundDB;
    private List<UI_StoreItem> _generatedButtons = new List<UI_StoreItem>();
    private int _currentTabIndex = 0;

    private GridLayoutGroup _currentGridLayout;
    private float _viewportHeight;

    /// <summary>
    /// UIManager가 ShowPopupUI()로 생성할 때 호출
    /// </summary>
    public override void Init()
    {
        base.Init(); // (부모의 SetCanvas() 호출)

        _gameManager = Managers.Instance.Game;
        _storeManager = Managers.Instance.Store;
        _soundManager = Managers.Instance.Sound;
        _skinDB = Managers.Instance.SkinDB;
        _badBlockDB = Managers.Instance.BadBlockDB;
        _backgroundDB = Managers.Instance.BackgroundDB;

        if (scrollRect != null)
        {
            _viewportHeight = (scrollRect.transform as RectTransform).rect.height;
        }

        // (리스너 연결)
        closeStoreButton.onClick.AddListener(OnCloseStorePressed);
        tabBlockSkin.onClick.AddListener(() => OnTabPressed(0));
        tabBadBlockSkin.onClick.AddListener(() => OnTabPressed(1));
        tabBackgroundSkin.onClick.AddListener(() => OnTabPressed(2));
        
        // (초기 상태)
        OnTabPressed(0); // (기본값: 블록 스킨 탭)
    }

    // --- 3. 탭 및 상점 로직 (v2.7) ---
    private void OnCloseStorePressed()
    {
        _soundManager.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
        Managers.Instance.UI.ClosePopupUI(this); // (자신을 닫음)
        Managers.Instance.UI.ShowTitlePanel(); // (v2.7) UIManager에게 타이틀 복귀 요청
    }

    private void OnTabPressed(int tabIndex)
    {
        _currentTabIndex = tabIndex;
        
        // (v2.7) 탭 활성화/비활성화 (HTML 목업의 tab-active 구현)
        tabBlockSkin.interactable = (tabIndex != 0);
        tabBadBlockSkin.interactable = (tabIndex != 1);
        tabBackgroundSkin.interactable = (tabIndex != 2);
        
        // (v2.7) 패널 활성화/비활성화
        panelBlockSkin.gameObject.SetActive(tabIndex == 0);
        panelBadBlockSkin.gameObject.SetActive(tabIndex == 1);
        panelBackgroundSkin.gameObject.SetActive(tabIndex == 2);
        
        switch (tabIndex)
        {
            case 0:
                scrollRect.content = panelBlockSkin;
                _currentGridLayout = panelBlockSkin.GetComponent<GridLayoutGroup>();
                break;
            case 1:
                scrollRect.content = panelBadBlockSkin;
                _currentGridLayout = panelBadBlockSkin.GetComponent<GridLayoutGroup>();
                break;
            case 2:
                scrollRect.content = panelBackgroundSkin;
                _currentGridLayout = panelBackgroundSkin.GetComponent<GridLayoutGroup>();
                break;
        }

        GenerateStoreItems();
    }

    /// <summary>
    /// [핵심] (지적 3) 현재 탭에 맞는 DB를 읽어 아이템 프리팹을 '자동 생성'
    /// </summary>
    private void GenerateStoreItems()
    {     
        RectTransform currentGridRoot = scrollRect.content;
        int itemCount = 0;
        
        // 1. 어떤 DB와 그리드를 사용할지 결정
        switch (_currentTabIndex)
        {
            case 0: // 블록 스킨
                if (_skinDB != null) itemCount = _skinDB.skins.Count;
                break;
            case 1: // 방해 블록 스킨
                if (_badBlockDB != null) itemCount = _badBlockDB.badBlocks.Count; // [수정]
                break;
            case 2: // 배경 스킨
                if (_backgroundDB != null) itemCount = _backgroundDB.backgrounds.Count; // [수정]
                break;
        }

        // 2. 기존 버튼 파괴
        foreach (Transform child in currentGridRoot)
        {
            Destroy(child.gameObject);
        }
        _generatedButtons.Clear();

        // 3. DB 개수만큼 프리팹 생성
        for (int i = 0; i < itemCount; i++)
        {
            GameObject go = Instantiate(storeItemPrefab, currentGridRoot);
            UI_StoreItem skinButton = go.GetComponent<UI_StoreItem>();
            skinButton.Init(_currentTabIndex, i); 
            _generatedButtons.Add(skinButton);
        }
        CalculateContentHeight(currentGridRoot, itemCount);
        UpdateStoreUI(); 
    }
    
    private void CalculateContentHeight(RectTransform contentRect, int itemCount)
    {
        if (_currentGridLayout == null || itemCount == 0)
        {
            // 아이템이 없으면 '기본 크기' (Viewport 높이)로(로) 설정
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, _viewportHeight);
            return;
        }

        int columnCount = _currentGridLayout.constraintCount; 
        int totalRows = Mathf.CeilToInt((float)itemCount / columnCount);
        float paddingTop = _currentGridLayout.padding.top;
        float paddingBottom = _currentGridLayout.padding.bottom;
        float cellHeight = _currentGridLayout.cellSize.y;
        float spacingY = _currentGridLayout.spacing.y;
        float contentHeight = paddingTop + paddingBottom + (totalRows * cellHeight) + ((totalRows - 1) * spacingY);

        float newHeight = Mathf.Max(_viewportHeight, contentHeight);
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);
        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
    }   
    /// <summary>
    /// (v2.7) UI_StoreItem.cs가          
    /// </summary>
    public void OnStoreItemPressed(int category, int index)
    {
        _soundManager.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
        
        switch (category)
        {
            case 0: // 블록 스킨
                if (_storeManager.IsSkinUnlocked(index)) { 
                    _storeManager.EquipSkin(index);
                } else {
                    if (_storeManager.BuySkin(index)) { _soundManager.Play(Define.SoundID.SFX_Coin, Define.Sound.Effect); }
                    else { storeCoinText.transform.DOKill(); storeCoinText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f); }
                }
                break;
            case 1: // 방해 블록 스킨
                if (_storeManager.IsBadBlockSkinUnlocked(index)) { 
                    _storeManager.EquipBadBlockSkin(index); 
                } else {
                    if (_storeManager.BuyBadBlockSkin(index)) { _soundManager.Play(Define.SoundID.SFX_Coin, Define.Sound.Effect); } 
                    else { storeCoinText.transform.DOKill(); storeCoinText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f); }
                }
                break;
            case 2: // 배경 스킨
                if (_storeManager.IsBackgroundSkinUnlocked(index)) { 
                    _storeManager.EquipBackgroundSkin(index); 
                } else {
                    if (_storeManager.BuyBackgroundSkin(index)) { _soundManager.Play(Define.SoundID.SFX_Coin, Define.Sound.Effect); } 
                    else { storeCoinText.transform.DOKill(); storeCoinText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f); }
                }
                break;
        }
        
        UpdateStoreUI();
    }
    
    /// <summary>
    /// (v2.7) 자동 생성된 버튼들의 상태를 갱신
    /// </summary>
    private void UpdateStoreUI()
    {
        // [수정 v2.19] StoreManager 참조
        UpdateCoinText(_storeManager.GetTotalCoins());

        switch (_currentTabIndex)
        {
            case 0: UpdateStoreItems_BlockSkin(); break;
            case 1: UpdateStoreItems_BadBlock(); break;
            case 2: UpdateStoreItems_Background(); break;
        }
    }
    private void UpdateStoreItems_BlockSkin()
    {
        if (_skinDB == null) return;

        for (int i = 0; i < _generatedButtons.Count; i++) 
        {
            if (i >= _skinDB.skins.Count) continue; 

            SkinEntry entry = _skinDB.skins[i];
            string textToShow;
            bool interactable;
            bool showCoinIcon = false;
            

            if (_storeManager.IsSkinUnlocked(i)) {
                if (Managers.Instance.Game.currentSkinIndex == i) { 
                    textToShow = "장착함"; 
                    interactable = false; 
                }
                else { 
                    textToShow = "장착"; 
                    interactable = true; 
                }
            } else {
                textToShow = $"{entry.price}"; // "100"
                interactable = true; 
                showCoinIcon = true;
            }
            
            _generatedButtons[i].SetState(textToShow, interactable, showCoinIcon);
            _generatedButtons[i].SetData(entry.frameSprite, entry.displayName);
        }
    }

    private void UpdateStoreItems_BadBlock()
    {
        if (_badBlockDB == null) return;

        for (int i = 0; i < _generatedButtons.Count; i++)
        {
            if (i >= _badBlockDB.badBlocks.Count) continue;
            
            BadBlockEntry entry = _badBlockDB.badBlocks[i];
            string textToShow;
            bool interactable;
            bool showCoinIcon = false;

            // [수정] GameManager의 새 함수 사용
            if (_storeManager.IsBadBlockSkinUnlocked(i)) {
                if (Managers.Instance.Game.currentBadBlockSkinIndex == i) { 
                    textToShow = "장착됨"; 
                    interactable = false; 
                }
                else { 
                    textToShow = "장착"; 
                    interactable = true; 
                }
            } else {
                textToShow = $"{entry.price}"; 
                interactable = true; 
                showCoinIcon = true;
            }
            
            _generatedButtons[i].SetState(textToShow, interactable, showCoinIcon);
            _generatedButtons[i].SetData(entry.badBlockSprite, entry.displayName);
        }
    }

    private void UpdateStoreItems_Background()
    {
        if (_backgroundDB == null) return;

        for (int i = 0; i < _generatedButtons.Count; i++)
        {
            if (i >= _backgroundDB.backgrounds.Count) continue;
            
            BackgroundEntry entry = _backgroundDB.backgrounds[i];
            string textToShow;
            bool interactable;
            bool showCoinIcon = false;

            if (_storeManager.IsBackgroundSkinUnlocked(i)) {
                if (Managers.Instance.Game.currentBackgroundSkinIndex == i) { // [수정]
                    textToShow = "장착됨"; 
                    interactable = false; 
                }
                else { 
                    textToShow = "장착"; 
                    interactable = true; 
                }
            } else {
                textToShow = $"{entry.price}"; 
                interactable = true; 
                showCoinIcon = true;
            }
            
            _generatedButtons[i].SetState(textToShow, interactable, showCoinIcon);
            _generatedButtons[i].SetData(entry.backgroundSprite, entry.displayName);
        }
    }

    // (v2.7) 코인 텍스트 갱신
    public void UpdateCoinText(int totalCoins)
    {
        if (storeCoinText != null) storeCoinText.text = totalCoins.ToString();
    }
}