using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// (리빌드 v2.5 - 최종본)
/// (v2.5: 유저님의 아키텍처 + Shape Fit Rush 매니저 통합)
/// (v2.5: SceneManagerEx -> SceneManager(Mono)로 변경)
/// </summary>
public class Managers : MonoBehaviour
{
    static Managers s_instance; 
    public static Managers Instance { get { return s_instance; } }

    // --- 1. 데이터 (SO) ---
    public GameSettings Settings { get; private set; }
    public SkinDatabase SkinDB { get; private set; }
    public BackgroundDatabase BackgroundDB { get; private set; }
    public BadBlockDatabase BadBlockDB { get; private set; }
    public SoundDatabase SoundDB { get; private set; }
    public UIPrefabDatabase PopupDB { get; private set; }

    #region Core (MonoBehaviour)
    public PoolManager Pool { get; private set; }
    public SceneManager Scene { get; private set; }
    public SoundManager Sound { get; private set; }
    public AdsManager Ads { get; private set; }
    public ResourceManager Resource { get; private set; }

    #endregion

    #region Contents (MonoBehaviour)
    public GameManager Game { get; private set; }
    public UIManager UI { get; private set; }
    public DataManager Data { get; private set; }
    public StoreManager Store { get; private set; }
    public JudgmentManager Judgment { get; private set; }
    #endregion 

	public async UniTask Init(Canvas loadingCanvas, Slider progressBar)
    {
        if (s_instance != null) { Destroy(this.gameObject); return; }
        s_instance = this;

        Pool = GetComponent<PoolManager>();
        Scene = GetComponent<SceneManager>();
        Sound = GetComponent<SoundManager>();
        Ads = GetComponent<AdsManager>();
        Resource = GetComponent<ResourceManager>(); 
        Data = GetComponent<DataManager>();
        Game = GetComponent<GameManager>();
        UI = GetComponent<UIManager>();
        Store = GetComponent<StoreManager>(); 
        Judgment = GetComponent<JudgmentManager>();

        // 씬 매니저 UI 설정
        Scene.SetupLoadingUI(progressBar, loadingCanvas);
        
        // 모든 매니저 초기화
        Resource.Init(); 
        await Pool.Init();
        Data.Init(); 
        Sound.Init();
        Ads.Init();
        UI.Init(); 
        Game.Init();
        Store.Init();

        await Judgment.Init();

        Debug.Log("--- All Managers Initialized (v2.6) ---");
    }

    /// <summary>
    /// (v2.6) Bootstrapper가 호출 (데이터 등록)
    /// </summary>
    public void RegisterData(GameSettings settings) { this.Settings = settings; }
    public void RegisterData(SkinDatabase skinDB) { this.SkinDB = skinDB; }
    public void RegisterData(BackgroundDatabase bgDB) { this.BackgroundDB = bgDB; }
    public void RegisterData(BadBlockDatabase bbDB) { this.BadBlockDB = bbDB; }
    public void RegisterData(SoundDatabase soundDB) { this.SoundDB = soundDB; }
    public void RegisterData(UIPrefabDatabase popupDB) { this.PopupDB = popupDB; }
    // (v2.5) 씬 전환 시 호출
    public static void Clear()
    {
        if (s_instance == null) return;
        Instance.UI.Clear();
    }
}