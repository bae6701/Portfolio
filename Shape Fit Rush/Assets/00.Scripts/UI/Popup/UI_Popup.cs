using UnityEngine;

/// <summary>
/// [2주차 v2.7] 팝업 UI (예: StorePanel, SettingsPanel)
/// (v2.5 UIManager.cs가 참조)
/// </summary>
public class UI_Popup : UI_Base
{
    public override void Init()
    {
        // (v2.7) 캔버스 설정 (팝업은 겹쳐야 하므로 sort = true)
        Managers.Instance.UI.SetCanvas(gameObject, sort: true);
    }

    // (v2.7) 팝업 닫기 버튼
    public virtual void ClosePopupUI()
    {
        Managers.Instance.UI.ClosePopupUI(this);
    }
}