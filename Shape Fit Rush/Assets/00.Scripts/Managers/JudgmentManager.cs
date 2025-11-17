using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// [3주차 v2.19 - 현업 방식]
/// (GameManager에서 '판정/규칙/입력' 책임을 분리)
/// </summary>
public class JudgmentManager : MonoBehaviour
{
    // --- 1. 씬(Scene) 참조 ---
    public JudgmentZone JudgmentZone { get; private set; }
    
    // --- 2. VFX 캐시 ---
    private GameObject vfxPerfect, vfxGood, vfxMiss, vfxFeverAura;

    public async UniTask Init()
    {
        vfxPerfect = await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_Perfect");
        vfxGood = await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_Good");
        vfxMiss = await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_Miss");
        vfxFeverAura = await Managers.Instance.Resource.LoadAsync<GameObject>("VFX_FeverAura");
        
        if (Managers.Instance.Game != null)
        {
            Managers.Instance.Game.OnSkinChanged += OnSkinChanged;
        }
    }

    void OnDestroy()
    {
        if (Managers.Instance != null && Managers.Instance.Game != null)
        {
            Managers.Instance.Game.OnSkinChanged -= OnSkinChanged;
        }
    }

    public void RegisterSceneComponents(JudgmentZone zone)
    {
        this.JudgmentZone = zone;
    }

    private void OnSkinChanged(SkinEntry entry)
    {
        if (JudgmentZone != null && entry != null)
        {
            JudgmentZone.SetSkin(entry.frameSprite);
        }
    }
    /// <summary>
    /// GameManager.Update()가 매 프레임 호출 (입력 처리)
    /// </summary>
    public void Update()
    { 
        // (v2.20) 터치/마우스 통합 입력
        if (Input.GetMouseButtonDown(0))
        {
            // (v2.21) UI 클릭 방지
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            ProcessTap();
        }
    }

    /// <summary>
    /// 판정 로직 (GameManager.CheckJudgment에서 이관)
    /// </summary>
    public void ProcessTap()
    {
        GameManager game = Managers.Instance.Game;
        if (game.IsGameOver) return;
        if (game.isFeverMode) return; 
        if (JudgmentZone == null) return;
        
        Block currentBlock = JudgmentZone.currentBlockInZone; 

        if (currentBlock == null) 
        { 
            game.TriggerMiss(); // (결과만 GameManager에 '보고')
            return; 
        }
        
        Poolable poolable = currentBlock.GetComponent<Poolable>();
        if (poolable == null) { return; }

        if (!currentBlock.isGoodBlock) 
        { 
            game.TriggerMiss(); 
            Managers.Instance.Pool.Push(poolable); 
            return; 
        }

        float distance = 0f;
        if (currentBlock.direction == Block.MoveDirection.Down) { distance = Mathf.Abs(currentBlock.transform.position.y - JudgmentZone.transform.position.y); }
        else { distance = Mathf.Abs(currentBlock.transform.position.x - JudgmentZone.transform.position.x); }

        // (v2.19) 판정 결과를 GameManager에 '보고'
        if (distance <= Managers.Instance.Settings.perfectThreshold) game.TriggerPerfect();
        else if (distance <= Managers.Instance.Settings.goodThreshold) game.TriggerGood();
        else game.TriggerMiss();

        JudgmentZone.currentBlockInZone = null;
        Managers.Instance.Pool.Push(poolable); 
    }
    
    // --- VFX/SFX 처리 (GameManager에서 이관) ---
    public void PopVFX(Define.SoundID soundID)
    {
        GameObject vfxPrefab = null;
        switch (soundID)
        {
            case Define.SoundID.SFX_Perfect: vfxPrefab = vfxPerfect; break;
            case Define.SoundID.SFX_Good: vfxPrefab = vfxGood; break;
            case Define.SoundID.SFX_Miss: vfxPrefab = vfxMiss; break;
        }
        
        if (vfxPrefab == null || JudgmentZone == null) return;
        
        Poolable poolable = Managers.Instance.Pool.Pop(vfxPrefab);
        if (poolable != null) { poolable.transform.position = JudgmentZone.transform.position; }
    }
    
    public GameObject PopFeverAura()
    {
        if (vfxFeverAura == null || JudgmentZone == null) return null;
        
        GameObject go = Managers.Instance.Pool.Pop(vfxFeverAura).gameObject;
        go.transform.SetParent(JudgmentZone.transform);
        go.transform.localPosition = Vector3.zero;
        return go;
    }
}