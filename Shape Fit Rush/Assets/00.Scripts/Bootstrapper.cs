using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// [1주차 v2.6] (지적 1, 2, 3 해결)
/// 게임 시작 시 '최초 1회' 실행되는 부트스트래퍼.
/// (1)데이터(SO) 비동기 로드 (2)@Managers 생성 (3)매니저 생성/등록
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    [Header("Loading UI")]
    public Slider progressBar; 
    public Canvas loadingCanvas; 
    
    private const float BOOTSTRAP_PROGRESS = 0.7f; // (부트스트랩 지분 70%)
    private const float SCENE_LOAD_PROGRESS = 0.3f;

    async UniTaskVoid Start()
    {
        try
        {
            await Bootstrap();
        }
        catch (Exception e)
        {
            Debug.LogError($"부트스트랩 실패: {e.Message}");
            // (필요 시, 유저에게 에러 팝업을 띄우는 로직)
        }
    }
    private async UniTask Bootstrap()
    {
        if (Managers.Instance != null)
            Destroy(Managers.Instance.gameObject);
            
        progressBar.value = 0.07f;

        var managersPrefabHandle = Addressables.LoadAssetAsync<GameObject>("@Managers");
        GameObject managersPrefab = await managersPrefabHandle;

        if (managersPrefab == null)
        {
            Debug.LogError("!!! @Managers.prefab을(를) 로드할 수 없습니다.");
            return;
        }

        GameObject managersGO = Instantiate(managersPrefab);
        managersGO.name = "@Managers";
        DontDestroyOnLoad(managersGO);
        
        Managers managers = managersGO.AddComponent<Managers>();
        progressBar.value = 0.14f;

        // --- 3. (v3.1) Addressables 비동기 로드 (UniTask) ---
        
        // [수정 v3.1] await handle.Task (Task) -> await handle (UniTask)
        
        managers.RegisterData(await Addressables.LoadAssetAsync<GameSettings>("GameSettings"));
        managers.RegisterData(await Addressables.LoadAssetAsync<SkinDatabase>("SkinDatabase"));
        managers.RegisterData(await Addressables.LoadAssetAsync<BackgroundDatabase>("BackgroundDatabase"));
        managers.RegisterData(await Addressables.LoadAssetAsync<BadBlockDatabase>("BadBlockDatabase"));
        managers.RegisterData(await Addressables.LoadAssetAsync<SoundDatabase>("SoundDatabase"));
        managers.RegisterData(await Addressables.LoadAssetAsync<UIPrefabDatabase>("UIPrefabDatabase"));

        progressBar.value = 0.56f;

        // (v3.0) 모든 DB 로드가 완료된 후 Init() 호출
        await managers.Init(loadingCanvas, progressBar); 
        progressBar.value = BOOTSTRAP_PROGRESS;

        // (v3.0) Init()이 끝난 후 씬(Scene) 로드
        await managers.Scene.LoadSceneAsync(Define.Scene.MainGameScene, BOOTSTRAP_PROGRESS, SCENE_LOAD_PROGRESS);
    }
}
