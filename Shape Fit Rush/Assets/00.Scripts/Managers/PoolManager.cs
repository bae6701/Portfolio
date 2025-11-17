using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// [1주차 v2.6] (v2.5 아키텍처 호환)
/// (지적 3: Stack<T> 사용, 동적 확장)
/// (MonoBehaviour - Managers.cs가 생성)
/// </summary>
public class PoolManager : MonoBehaviour
{
	#region Pool (Inner Class)
	class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }
        Stack<Poolable> _poolStack = new Stack<Poolable>();
        List<Poolable> _activeList = new List<Poolable>();
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)
                Push(Create());
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>(); 
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null) return;
            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;
            _poolStack.Push(poolable);
            _activeList.Remove(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;
            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
            {
                // (지적 3) 풀이 비었으면 5개 배치 생성
                for (int i = 0; i < 5; i++)
                    Push(Create());
                poolable = _poolStack.Pop(); 
            }

            poolable.gameObject.SetActive(true);
            
            if (parent != null)
                poolable.transform.parent = parent;
            else
                poolable.transform.parent = Root; 

            poolable.IsUsing = true;
            _activeList.Add(poolable);

            return poolable;
        }

        /// <summary>
        /// 현재 활성화된(Pop된) 모든 오브젝트를 강제로 Push
        /// </summary>
        public void ClearActive()
        {
            // (리스트를 뒤에서부터 순회해야 안전하게 제거 가능)
            for (int i = _activeList.Count - 1; i >= 0; i--)
            {
                Push(_activeList[i]);
            }
            _activeList.Clear();
        }
    }
	#endregion

	Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;

    public async UniTask Init()
    {
        if (_root == null)
        {
             _root = new GameObject { name = "@Pool_Root" }.transform;
             _root.SetParent(Managers.Instance.transform);
        }
        await WarmUpPools();

        if (Managers.Instance.Game != null)
        {
            Managers.Instance.Game.OnGameStart += OnGameReset;
            Managers.Instance.Game.OnGameOver += OnGameReset;
        }
    }

    void OnDestroy()
    {
        if (Managers.Instance != null && Managers.Instance.Game != null)
        {
            Managers.Instance.Game.OnGameStart -= OnGameReset;
            Managers.Instance.Game.OnGameOver -= OnGameReset;
        }
    }

    private void OnGameReset()
    {
        Debug.Log("--- PoolManager: Clearing All Active Objects ---");
        foreach (Pool pool in _pool.Values)
        {
            pool.ClearActive();
        }
    }
    private async UniTask WarmUpPools()
    {
        // 1. 스킨 프리팹 예열
        SkinDatabase skinDB = Managers.Instance.SkinDB;
        if (skinDB != null && skinDB.skins != null)
        {
            foreach (SkinEntry skin in skinDB.skins)
            {
                if (skin != null && skin.blockPrefab != null)
                    CreatePool(skin.blockPrefab, 10); 
            }
        }
        
        // 2. 방해 블록 예열
        GameObject badBlockPrefab = await Managers.Instance.Resource.LoadAsync<GameObject>("Block_Triangle");
        CreatePool(badBlockPrefab, 10);
        
        // 3. VFX 예열 (Addressables에서 로드)
        CreatePool(await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_Perfect"), 5);
        CreatePool(await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_Good"), 5);
        CreatePool(await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_Miss"), 5);
        CreatePool(await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_FeverAura"), 2);
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        if (original == null) return;
        if (_pool.ContainsKey(original.name)) return;

        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;
        _pool.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (original == null) return null;
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);
        return _pool[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Original;
    }
}