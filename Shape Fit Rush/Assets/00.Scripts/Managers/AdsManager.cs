using UnityEngine;
using UnityEngine.Advertisements; 
using System; 

/// <summary>
/// (리빌드 v2.5 최종본)
/// (v2.5: Managers.cs가 AddComponent로 생성)
/// (MonoBehaviour가 필수!)
/// </summary>
public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    // --- 2. 광고 유닛 ID ---
    [Header("Game IDs")]
    [SerializeField] string _androidGameId = "5880706"; 
    [SerializeField] string _iOSGameId = "5880707"; 
    
    [Header("Ad Unit IDs (Android)")]
    [SerializeField] string _rewardedUnitId = "Rewarded_Android"; 
    [SerializeField] string _interstitialUnitId = "Interstitial_Android"; 
    
    // [Header("Ad Unit IDs (iOS)")]
    // [SerializeField] string _rewardedUnitId_iOS = "Rewarded_iOS";
    // [SerializeField] string _interstitialUnitId_iOS = "Interstitial_iOS";

    private string _gameId;
    
    // --- 3. 콜백 (Callback) ---
    public Action OnRewardedAdCompleted; 

    // --- 4. 로드 상태 ---
    private bool isRewardedAdLoaded = false;
    public bool CheckRewardedAdLoaded() { return isRewardedAdLoaded; }

    public void Init() 
    {
        #if UNITY_IOS
            _gameId = _iOSGameId;
            _rewardedUnitId = _rewardedUnitId_iOS;
            _interstitialUnitId = _interstitialUnitId_iOS;
        #elif UNITY_ANDROID
            _gameId = _androidGameId;
        #elif UNITY_EDITOR
            _gameId = _androidGameId;
        #endif
        
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, true, this);
        }
    }
    
    private void LoadAds()
    {
        Debug.Log("Loading Ads...");
        Advertisement.Load(_rewardedUnitId, this); 
        Advertisement.Load(_interstitialUnitId, this);
    }

    // --- 6. IUnityAdsInitializationListener ---
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads Initialization Complete.");
        LoadAds(); 
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
    }

    // --- 7. IUnityAdsLoadListener ---
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad Loaded: {placementId}");
        if (placementId.Equals(_rewardedUnitId))
        {
            isRewardedAdLoaded = true;
        }
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit {placementId}: {error} - {message}");
        if (placementId.Equals(_rewardedUnitId))
        {
            isRewardedAdLoaded = false;
        }
    }

    // --- 8. 광고 재생 (Show) ---
    public void ShowRewardedAd()
    {
        Debug.Log($"Showing Rewarded Ad: {_rewardedUnitId}");
        Advertisement.Show(_rewardedUnitId, this); 
    }
    public void ShowInterstitialAd()
    {
        Debug.Log($"Showing Interstitial Ad: {_interstitialUnitId}");
        Advertisement.Show(_interstitialUnitId, this);
    }
    
    // --- 9. IUnityAdsShowListener ---
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {placementId}: {error} - {message}");
    }
    public void OnUnityAdsShowStart(string placementId) 
    {
        Debug.Log($"Ad Show Start: {placementId}");
    }
    public void OnUnityAdsShowClick(string placementId) 
    {
        Debug.Log($"Ad Show Click: {placementId}");
    }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Ad Show Complete: {placementId}");
        
        if (placementId.Equals(_rewardedUnitId))
        {
            isRewardedAdLoaded = false; 
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Reward Earned!");
                OnRewardedAdCompleted?.Invoke();
            }
            Advertisement.Load(_rewardedUnitId, this); 
        }
        else if (placementId.Equals(_interstitialUnitId))
        {
            Advertisement.Load(_interstitialUnitId, this);
        }
    }
}