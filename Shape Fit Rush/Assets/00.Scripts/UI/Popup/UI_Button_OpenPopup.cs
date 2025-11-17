using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [3주차 v2.21 - 현업 방식]
/// (UIManager를(을) 직접 호출하는 대신, 'UIPopupID' 에셋을(을) 참조하여
///  어떤 팝업을 띄울지 '이벤트'만 발생시키는 버튼)
/// </summary>
[RequireComponent(typeof(Button))]
public class UI_Button_OpenPopup : MonoBehaviour
{
    [Header("Popup ID Asset")]
    [SerializeField] private UIPopupID popupToOpen; // (인스펙터에서 .asset 파일 연결)

    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenPopup);
    }

    private void OpenPopup()
    {
        if(popupToOpen == null)
        {
            Debug.LogWarning("UIPopupID is not assigned in " + gameObject.name);
            return;
        }   
        if (popupToOpen != null && Managers.Instance != null && Managers.Instance.UI != null)
        {
            Managers.Instance.UI.ShowPopupUI(popupToOpen);
        }
    }
}