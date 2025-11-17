using UnityEngine;

/// <summary>
/// [1주차 v2.6] (지적 2 해결)
/// MainGameScene에 배치되어,
/// 씬(Scene)의 핵심 컴포넌트(Spawner, Zone)를 GameManager에 '등록'합니다.
/// </summary>
public class GameRoot : MonoBehaviour
{
    [Header("Scene Components")]
    public BlockSpawner spawner;
    public JudgmentZone judgmentZone;
    
    void Start()
    {
        if (Managers.Instance != null)
        {
            // [수정 v2.19] JudgmentManager에게 JudgmentZone을 등록
            if (Managers.Instance.Judgment != null)
                Managers.Instance.Judgment.RegisterSceneComponents(judgmentZone);
        }
    }
}