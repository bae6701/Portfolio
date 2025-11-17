using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// [1주차 v2.7] (지적 2, 3 해결)
/// MainGameScene의 Canvas에 부착되어,
/// UIManager에게 '정적(Static)' UI와 '자동 생성(Prefab)' UI를 전달합니다.
/// </summary>
public class UI_SceneRoot : MonoBehaviour
{
    [Header("Static UI (for UIManager)")]
    public GameObject titlePanel; 
    public GameObject resultPanel;
    public GameObject inGameUIPanel;

    [Header("Background Image")]
    public Image inGameBackgroundImage;

    [Header("In-Game Texts")]
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text timeText;
    public TMP_Text inGameCoinText; 
    
    [Header("Result Texts")]
    public TMP_Text resultScoreText;
    public TMP_Text resultTimeText;
    public TMP_Text resultHighScoreText;
    public TMP_Text resultBestTimeText;

    [Header("Buttons")]
    public Button startButton; 
    public Button retryButton;
    public Button homeButton;
    public Button openStoreButton; // (타이틀의 상점 열기 버튼)
    public Button continueButton;

    void Start()
    {
        if (Managers.Instance != null && Managers.Instance.UI != null)
        {
            Managers.Instance.UI.RegisterSceneUI(this);
        }
    }
}