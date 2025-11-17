using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{

    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();
    
    // (v3.1) TDD 원칙: Instantiate된 객체 핸들 관리
    private Dictionary<GameObject, AsyncOperationHandle<GameObject>> _instanceHandles = new Dictionary<GameObject, AsyncOperationHandle<GameObject>>();
    public void Init(){}
    public async UniTask<T> LoadAsync<T>(string key) where T : Object
    {
        // 1. 이미 로드된 핸들이 있는지 확인
        if (_handles.TryGetValue(key, out var handle))
        {
            return handle.Result as T;
        }

        // 2. 새로 로드
        var newHandle = Addressables.LoadAssetAsync<T>(key);
        _handles.Add(key, newHandle); // (참조 카운트 1 증가)
        
        await newHandle.ToUniTask(); // UniTask로 대기
        
        if (newHandle.Status == AsyncOperationStatus.Succeeded)
        {
            return newHandle.Result;
        }
        else
        {
            Debug.LogError($"[ResourceManager] 에셋 로드 실패: {key}");
            _handles.Remove(key);
            return null;
        }
    }

    public async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null)
    {
        // (v3.1) PoolManager 연동은 다음 단계. 우선 Addressables로 생성.
        var handle = Addressables.InstantiateAsync(key, parent);
        await handle.ToUniTask();
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // (TDD 원칙: 생성된 객체와 핸들을 매핑하여 추적)
            _instanceHandles.Add(handle.Result, handle);
            return handle.Result;
        }
        else
        {
            Debug.LogError($"[ResourceManager] 프리팹 Instantiate 실패: {key}");
            return null;
        }
    }

    public void Destroy(GameObject go)
    {
        if (go == null) return;
        
        // (v3.1) Poolable 연동 (v3.2)
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Instance.Pool.Push(poolable);
            return;
        }

        // (v3.1) Addressables로 생성된 객체인지 확인하고 릴리즈
        if (_instanceHandles.TryGetValue(go, out var handle))
        {
            Addressables.ReleaseInstance(handle);
            _instanceHandles.Remove(go);
        }
        else
        {
            // Addressables로 만든게 아니면 그냥 파괴
            Object.Destroy(go);
        }
    }
    public void Release(string key)
    {
        if (_handles.TryGetValue(key, out var handle))
        {
            Addressables.Release(handle);
            _handles.Remove(key);
        }
    }
}