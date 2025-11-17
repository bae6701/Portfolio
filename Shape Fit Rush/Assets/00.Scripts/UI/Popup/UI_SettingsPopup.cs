using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [3주차 v2.8] (지적 4)
/// 설정 팝업(BGM/SFX)을 관리하는 UI 스크립트.
/// (UI_Popup을 상속)
/// </summary>
public class UI_SettingsPopup : UI_Popup
{
    [Header("UI Components")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button bgmButton; // (Music/On-Off 오브젝트 )
    [SerializeField] private Button sfxButton; // (Volume/On-Off 오브젝트 )

    [Header("Visuals (Icons)")]
    [SerializeField] private Image bgmIcon; // (Music/MusicTexture )
    [SerializeField] private Image sfxIcon; // (Volume/VolumeTexture )

    [Header("Visuals (On/Off Images)")]
    [SerializeField] private Image bgmOnOffImage;
    [SerializeField] private Image sfxOnOffImage; 
    
    [Header("Sprite Assets (Project 폴더에서 연결)")]
    [SerializeField] private Sprite iconBgmOn;
    [SerializeField] private Sprite iconBgmOff;
    [SerializeField] private Sprite iconSfxOn;
    [SerializeField] private Sprite iconSfxOff;
    [SerializeField] private Sprite spriteButtonOn;
    [SerializeField] private Sprite spriteButtonOff;

    private GameData _data;

    /// <summary>
    /// UIManager가 ShowPopupUI()로 생성할 때 호출
    /// </summary>
    public override void Init()
    {
        base.Init(); 
        _data = Managers.Instance.Data.GameData;

        // 1. 리스너 연결
        closeButton.onClick.AddListener(OnClosePressed);
        bgmButton.onClick.AddListener(OnBgmToggle);
        sfxButton.onClick.AddListener(OnSfxToggle);

        // 2. 현재 저장된 설정값으로 비주얼 초기화
        UpdateBgmVisual();
        UpdateSfxVisual();
    }

    private void OnClosePressed()
    {
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
        Managers.Instance.UI.ClosePopupUI(this); // (자신을 닫음)
    }

    private void OnBgmToggle()
    {
        _data.isBgmOn = !_data.isBgmOn;
        Managers.Instance.Sound.MuteBGM(!_data.isBgmOn);
        Managers.Instance.Data.SaveGame();
        
        // 2. 비주얼 갱신
        UpdateBgmVisual();
    }

    private void OnSfxToggle()
    {
        _data.isSfxOn = !_data.isSfxOn;
        Managers.Instance.Sound.MuteSFX(!_data.isSfxOn);
        Managers.Instance.Data.SaveGame();
        
        // 2. 비주얼 갱신 및 효과음 테스트
        UpdateSfxVisual();
        Managers.Instance.Sound.Play(Define.SoundID.SFX_Tap, Define.Sound.Effect);
    }

    private void UpdateBgmVisual()
    {
        if (_data.isBgmOn)
        {
            bgmIcon.sprite = iconBgmOn;
            bgmOnOffImage.sprite = spriteButtonOn;
        }
        else
        {
            bgmIcon.sprite = iconBgmOff;
            bgmOnOffImage.sprite = spriteButtonOff;
        }
    }
    
    private void UpdateSfxVisual()
    {
        if (_data.isSfxOn)
        {
            sfxIcon.sprite = iconSfxOn;
            sfxOnOffImage.sprite = spriteButtonOn;
        }
        else
        {
            sfxIcon.sprite = iconSfxOff;
            sfxOnOffImage.sprite = spriteButtonOff;
        }
    }
}