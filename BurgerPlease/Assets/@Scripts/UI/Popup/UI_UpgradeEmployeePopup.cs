using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeEmployeePopup : MonoBehaviour
{
	[SerializeField] Button _closeButton;
	[SerializeField] UI_UpgradeEmployeePopupItem _speedItem;
	[SerializeField] UI_UpgradeEmployeePopupItem _capacityItem;
	[SerializeField] UI_UpgradeEmployeePopupItem _hireItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		_closeButton.onClick.AddListener(OnClickCloseButton);

		_hireItem.SetInfo(EUpgradeEmployeePopupItemType.Hire, 1000);
    }

	void OnClickCloseButton()
	{
		gameObject.SetActive(false);
	}

}
