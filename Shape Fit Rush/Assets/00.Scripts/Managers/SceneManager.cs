using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

/// <summary>
/// [1주차 v2.6] 씬 로드와 ProgressBar 표시 전담
/// (MonoBehaviour - Managers.cs가 생성)
/// </summary>
public class SceneManager : MonoBehaviour 
{
    public Define.Scene CurrentSceneType { get; private set; } = Define.Scene.Unknown;

    private Slider _progressBar;
    private Canvas _loadingCanvas;
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void SetupLoadingUI(Slider progressBar, Canvas loadingCanvas)
    {
        _progressBar = progressBar;
        _loadingCanvas = loadingCanvas;
    }

    public async UniTask LoadSceneAsync(Define.Scene scene, float startProgress = 0f, float progressWeight = 1f)
    {
        if (_loadingCanvas != null)
            _loadingCanvas.gameObject.SetActive(true);
            
        if (_progressBar != null)
            _progressBar.value = startProgress;

        string sceneName = GetSceneName(scene);
        
        try
        {
            var op = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, true);

            while (op.IsDone == false)
            {
                // [v3.5] (예: 0.7f + (씬(Scene) 진행률 * 0.3f))
                if (_progressBar != null)
                    _progressBar.value = startProgress + (op.PercentComplete * progressWeight);
                
                await UniTask.Yield();
            }

            // 씬(Scene) 로딩 완료 후 (100% 채우기)
            if (_progressBar != null)
                _progressBar.value = startProgress + progressWeight; 

            CurrentSceneType = scene;
        }
        catch (Exception e)
        {
            Debug.LogError($"씬(Scene) 로드 실패 ({sceneName}): {e.Message}");
        }
        finally
        {
            // 로딩 완료 후 UI 숨기기
            if (_loadingCanvas != null)
                _loadingCanvas.gameObject.SetActive(false);
        }
    }
}