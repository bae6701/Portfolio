using UnityEngine;

/// <summary>
/// (리빌드 1주차 최종본 v2.5)
/// (v2.5: ServiceLocator -> Managers.Instance로 호출)
/// </summary>
[RequireComponent(typeof(ParticleSystem))] 
public class AutoPushToPool : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (ps.main.duration <= 0)
        {
            PushBack(); 
            return;
        }
        Invoke(nameof(PushBack), ps.main.duration);
    }

    void PushBack()
    {
        CancelInvoke();
        
        // (신뢰성) 씬 종료 시 Managers가 먼저 파괴될 수 있음
        if (Managers.Instance != null) 
        {
            Poolable poolable = this.GetComponent<Poolable>();
            Managers.Instance.Pool.Push(poolable);
        }
    }
}