using UnityEngine;

[RequireComponent(typeof(WorkerInteraction))]
public class Office : UnlockableBase
{
	[SerializeField] private GameObject Wall;
    private void Start()
    {
		GetComponent<WorkerInteraction>().OnTriggerStart = OnEnterOffice;
		GetComponent<WorkerInteraction>().OnTriggerEnd = OnLeaveOffice;
    }

	private void OnEnable()
	{
		Wall.SetActive(false);
	}

	private void OnDisable()
	{
		Wall.SetActive(true);
	}

	public void OnEnterOffice(WorkerController wc)
	{
		GameManager.Instance.UpgradeEmployeePopup.gameObject.SetActive(true);
	}

	public void OnLeaveOffice(WorkerController wc)
	{
		GameManager.Instance.UpgradeEmployeePopup.gameObject.SetActive(false);
	}
}
