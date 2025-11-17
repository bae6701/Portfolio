using UnityEngine;

/// <summary>
/// [2주차 v2.7] 모든 UI 스크립트의 '최상위 부모'.
/// (v2.5 UIManager.cs가 참조)
/// </summary>
public abstract class UI_Base : MonoBehaviour
{
    // (v2.7) 2단계에서 UI 자동화(Find)를 위해 사용
    public abstract void Init(); 
}